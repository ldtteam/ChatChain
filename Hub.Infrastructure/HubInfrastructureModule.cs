using Autofac;
using Hub.Core.Interfaces.Gateways.Repositories;
using Hub.Infrastructure.Data.Redis.Repositories;

namespace Hub.Infrastructure
{
    public class HubInfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestsRepository>().As<IRequestsRepository>().InstancePerLifetimeScope();
        }
    }
}