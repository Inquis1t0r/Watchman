﻿using System;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Queries;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Commons;
using Watchman.DomainModel.DiscordServer.Commands;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.Discord.Areas.Administration.Services;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersRolesService _usersRolesService;
        public AdministrationController(IQueryBus queryBus, ICommandBus commandBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory, UsersRolesService usersRolesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersRolesService = usersRolesService;
        }

        [AdminCommand]
        [DiscordCommand("messages")]
        public async Task ReadUserMessages(DiscordRequest request, Contexts contexts)
        {
            var mention = request.GetMention();
            var selectedUser = _usersService.GetUserByMention(contexts.Server, mention);
            if (selectedUser == null)
            {
                throw new UserNotFoundException(mention);
            }

            var timeRange = request.GetPastTimeRange(defaultTime: TimeSpan.FromHours(1));
            var query = new GetUserMessagesQuery(contexts.Server.Id, selectedUser.Id)
            {
                SentDate = timeRange
            };
            var messages = _queryBus.Execute(query).Messages
                .OrderBy(x => x.SentAt)
                .ToList();

            var messagesService = _messagesServiceFactory.Create(contexts);
            var hasForceArgument = request.HasArgument("force") || request.HasArgument("f");

            if (messages.Count > 200 && !hasForceArgument)
            {
                await messagesService.SendResponse(x => x.NumberOfMessagesIsHuge(messages.Count), contexts);
                return;
            }

            if (!messages.Any())
            {
                await _directMessagesService.TrySendMessage(contexts.User.Id, x => x.UserDidntWriteAnyMessageInThisTime(selectedUser), contexts);
            }
            else
            {
                var header = $"Messages from user {selectedUser} starting at {timeRange.Start}";
                var lines = messages.Select(x => $"{x.SentAt:yyyy-MM-dd HH:mm:ss} {x.Author.Name}: {x.Content.Replace("```", "")}");
                var linesBuilder = new StringBuilder().PrintManyLines(lines.ToArray(), contentStyleBox: true);

                await _directMessagesService.TrySendMessage(contexts.User.Id, header);
                await _directMessagesService.TrySendMessage(contexts.User.Id, linesBuilder.ToString(), MessageType.BlockFormatted);
            }

            await messagesService.SendResponse(x => x.SentByDmMessagesOfAskedUser(messages.Count, selectedUser), contexts);
        }

        [AdminCommand]
        [DiscordCommand("set role")]
        public async Task SetRoleAsSafe(DiscordRequest request, Contexts contexts)
        {
            var args = request.Arguments.Skip(1).ToList(); // 1 args is string "role", so it's not needed
            if (args.Count < 2)
            {
                throw new NotEnoughArgumentsException();
            }
            if (args.HasDuplicates())
            {
                throw new ArgumentsDuplicatedException();
            }

            var lastArgument = args.Last().Value.ToLower();
            var argumentIsNotCorrect = lastArgument != "safe" && lastArgument != "unsafe";
            if (argumentIsNotCorrect)
            {
                throw new NotEnoughArgumentsException();
            }
            var isSafe = lastArgument switch
            {
                "safe" => true,
                "unsafe" => false,
                _ => throw new NotEnoughArgumentsException()
            };
            var commandRoles = args.Select(x => x.Value).SkipLast(1);
            var roleService = new SetRoleService(_messagesServiceFactory, _queryBus, _commandBus,  _usersRolesService);
            await roleService.SetRoleAsSafe(contexts, commandRoles, isSafe);

            var messageService = _messagesServiceFactory.Create(contexts);
            string msg = string.Join(", ",
                args.Select(x => x.Value).SkipLast(1)) + " have been set as " + lastArgument;
            await messageService.SendMessage(msg);
        }
    }
}
