using Api.Core.Interfaces.UseCases.Client;
using Api.Core.Interfaces.UseCases.ClientConfig;
using Api.Core.Interfaces.UseCases.Group;
using Api.Core.Interfaces.UseCases.Invite;
using Api.Core.Interfaces.UseCases.Organisation;
using Api.Core.Interfaces.UseCases.OrganisationUser;
using Api.Core.UseCases.Client;
using Api.Core.UseCases.ClientConfig;
using Api.Core.UseCases.Group;
using Api.Core.UseCases.Invite;
using Api.Core.UseCases.Organisation;
using Api.Core.UseCases.OrganisationUser;
using Autofac;

namespace Api.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Client
            builder.RegisterType<CreateClientUseCase>().As<ICreateClientUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<DeleteClientUseCase>().As<IDeleteClientUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetClientUseCase>().As<IGetClientUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetClientsUseCase>().As<IGetClientsUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateClientUseCase>().As<IUpdateClientUseCase>().InstancePerLifetimeScope();

            // Client Config
            builder.RegisterType<GetClientConfigUseCase>().As<IGetClientConfigUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateClientConfigUseCase>().As<IUpdateClientConfigUseCase>()
                .InstancePerLifetimeScope();

            // Group
            builder.RegisterType<CreateGroupUseCase>().As<ICreateGroupUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<DeleteGroupUseCase>().As<IDeleteGroupUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetGroupUseCase>().As<IGetGroupUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetGroupsUseCase>().As<IGetGroupsUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateGroupUseCase>().As<IUpdateGroupUseCase>().InstancePerLifetimeScope();

            // Invite
            builder.RegisterType<CreateInviteUseCase>().As<ICreateInviteUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UseInviteUseCase>().As<IUseInviteUseCase>().InstancePerLifetimeScope();

            // Organisation
            builder.RegisterType<CreateOrganisationUseCase>().As<ICreateOrganisationUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DeleteOrganisationUseCase>().As<IDeleteOrganisationUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<GetOrganisationUseCase>().As<IGetOrganisationUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetOrganisationsUseCase>().As<IGetOrganisationsUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateOrganisationUseCase>().As<IUpdateOrganisationUseCase>()
                .InstancePerLifetimeScope();

            // Organisation User
            builder.RegisterType<DeleteOrganisationUserUseCase>().As<IDeleteOrganisationUserUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<GetOrganisationUserUseCase>().As<IGetOrganisationUserUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<GetOrganisationUsersUseCase>().As<IGetOrganisationUsersUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<UpdateOrganisationUserUseCase>().As<IUpdateOrganisationUserUseCase>()
                .InstancePerLifetimeScope();
        }
    }
}