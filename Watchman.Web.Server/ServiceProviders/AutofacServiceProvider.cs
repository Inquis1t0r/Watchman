﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Watchman.Discord;
using Watchman.IoC;

namespace Watchman.Web.Server.ServiceProviders
{
    public class AutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly IConfiguration _configuration;

        public AutofacServiceProviderFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var containerModule = new ContainerModule(this._configuration.GetConnectionString("Mongo"));
            var builder = containerModule.GetBuilder();
            builder.Populate(services);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();

            _ = new WatchmanBot(new DiscordConfiguration
            {
                MongoDbConnectionString = this._configuration.GetConnectionString("Mongo"),
                Token = this._configuration["Discord:Token"]
            }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();

            container.Resolve<HangfireJobsService>().SetDefaultJobs(container);

            return new AutofacServiceProvider(container);
        }
    }

    public class AutofacServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public AutofacServiceProvider(IContainer container)
        {
            this._container = container;
        }

        public object GetService(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }
    }
}
