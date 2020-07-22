﻿using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Linq;
using Watchman.Discord.Areas.Commons;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteRequestParser
    {
        private readonly DiscordRequest _request;
        private readonly UsersService _usersService;
        private readonly Contexts _contexts;
        //TODO Regex detecting mention/Id
        public MuteRequestParser(DiscordRequest request, UsersService usersService, Contexts contexts)
        {
            this._request = request;
            this._usersService = usersService;
            this._contexts = contexts;
        }

        public UserContext GetUser()
        {
            var mention = this._request.GetMention();
            //TODO: Regex -> Bool -> userToMute (GetUserByMention/GetUserById)
            //var userToMute = this._usersService.GetUserByMention(this._contexts.Server, mention);
            var userToMute = this._usersService.GetUserById(this._contexts.Server, mention);

            if (userToMute == null)
            {
                throw new UserNotFoundException(mention);
            }
            return userToMute;
        }

        public MuteEvent GetMuteEvent(ulong userId, Contexts contexts, DiscordRequest request)
        {
            var reason = this._request.Arguments.FirstOrDefault(x => x.Name == "reason" || x.Name == "r")?.Value;
            var timeRange = request.GetFutureTimeRange(defaultTime: TimeSpan.FromHours(1));
            return new MuteEvent(userId, timeRange, reason, contexts.Server.Id);
        }
    }
}
