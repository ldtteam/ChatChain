using Autofac;
using Hub.Core.Interfaces.UseCases;
using Hub.Core.Interfaces.UseCases.Events;
using Hub.Core.Interfaces.UseCases.Stats;
using Hub.Core.Services;
using Hub.Core.UseCases;
using Hub.Core.UseCases.Events;
using Hub.Core.UseCases.Stats;

namespace Hub.Core
{
    public class HubCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventsService>().SingleInstance();

            builder.RegisterType<GenericMessageUseCase>().As<IGenericMessageUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetGroupsUseCase>().As<IGetGroupsUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetClientUseCase>().As<IGetClientUseCase>().InstancePerLifetimeScope();
            
            //Event Use Cases
            builder.RegisterType<ClientEventUseCase>().As<IClientEventUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UserEventUseCase>().As<IUserEventUseCase>().InstancePerLifetimeScope();
            
            //Stats Use Cases
            builder.RegisterType<StatsRequestUseCase>().As<IStatsRequestUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<StatsResponseUseCase>().As<IStatsResponseUseCase>().InstancePerLifetimeScope();
        }
    }
}