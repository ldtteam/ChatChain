using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.Services;
using Api.Infrastructure.Data.MongoDB.Repositories;
using Api.Infrastructure.Data.Redis.Repositories;
using Api.Infrastructure.Data.Services;
using Autofac;

namespace Api.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ClientConfigRepository>().As<IClientConfigRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ClientRepository>().As<IClientRepository>().InstancePerLifetimeScope();
            builder.RegisterType<IS4ClientRepository>().As<IIS4ClientRepository>().InstancePerLifetimeScope();
            builder.RegisterType<GroupRepository>().As<IGroupRepository>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationRepository>().As<IOrganisationRepository>().InstancePerLifetimeScope();

            builder.RegisterType<InviteRepository>().As<IInviteRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PasswordGenerator>().As<IPasswordGenerator>().InstancePerLifetimeScope();
        }
    }
}