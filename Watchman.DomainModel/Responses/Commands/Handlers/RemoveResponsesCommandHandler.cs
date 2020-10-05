﻿using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class RemoveResponsesCommandHandler : ICommandHandler<RemoveResponsesCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public RemoveResponsesCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(RemoveResponsesCommand command)
        {
            using var session = this._sessionFactory.Create();
            foreach (var response in command.ResponsesToRemove)
            {
                await session.DeleteAsync(response);
            }
        }
    }
}