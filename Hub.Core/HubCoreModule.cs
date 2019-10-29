using Autofac;
using Hub.Core.Interfaces.UseCases;
using Hub.Core.UseCases;

namespace Hub.Core
{
    public class HubCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GenericMessageUseCase>().As<IGenericMessageUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<ClientEventUseCase>().As<IClientEventUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UserEventUseCase>().As<IUserEventUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetGroupsUseCase>().As<IGetGroupsUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetClientUseCase>().As<IGetClientUseCase>().InstancePerLifetimeScope();
        }
    }
}