﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Discord.Areas.Responses.BotCommands;
using Serilog;
using DomainResponse = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Controllers
{
    public class ResponsesController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly Services.ResponsesService _responsesService;
        private readonly ResponsesMessageService _responsesMessageService;
        private readonly string[] _possibleArguments = { "all", "default", "custom" };

        public ResponsesController(MessagesServiceFactory messagesServiceFactory, Services.ResponsesService responsesService, ResponsesMessageService responsesMessageService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
            this._responsesMessageService = responsesMessageService;
        }

        [AdminCommand]
        public async Task AddResponse(AddResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (command.OnEvent == null || command.Message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(command.OnEvent);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }
            var responseForThisServer = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (responseForThisServer != null)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, command.OnEvent));
                return;
            }

            var defaultResponse = await this._responsesService.GetResponseByOnEvent(command.OnEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse.Message == command.Message)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, command.OnEvent));
                return;
            }

            await this._responsesService.AddResponse(command.OnEvent, command.Message, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, command.OnEvent));
        }

        [AdminCommand]
        public async Task UpdateResponse(UpdateResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (command.OnEvent == null || command.Message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }

            var defaultResponse = await this._responsesService.GetResponseByOnEvent(command.OnEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse.Message == command.Message)
            {
                await this._responsesService.RemoveResponse(response.OnEvent, response.ServerId);
                await messageService.SendResponse(x => x.ResponseTheSameAsDefault(contexts, command.OnEvent));
                return;
            }

            await this._responsesService.UpdateResponse(response.Id, command.Message);
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, command.OnEvent, response.Message, command.Message));
        }

        [AdminCommand]
        public async Task RemoveResponse(RemoveResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            if (command.OnEvent == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }
            await this._responsesService.RemoveResponse(command.OnEvent, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, command.OnEvent));
        }

        [AdminCommand]
        public async Task Responses(ResponsesCommand command, Contexts contexts)
        {
            var typeOfResponses = command.Type?.ToLowerInvariant();
            if (!this._possibleArguments.Contains(typeOfResponses))
            {
                typeOfResponses = "all";
            }
            await this._responsesMessageService.PrintResponses(typeOfResponses, contexts);
        }
    }
}
