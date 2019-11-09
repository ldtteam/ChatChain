using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.OrganisationUser;

namespace Api.Core.UseCases.OrganisationUser
{
    public class DeleteOrganisationUserUseCase : IDeleteOrganisationUserUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public DeleteOrganisationUserUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(DeleteOrganisationUserRequest message,
            IOutputPort<DeleteOrganisationUserResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new DeleteOrganisationUserResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.DeleteOrgUsers))
                {
                    outputPort.Handle(new DeleteOrganisationUserResponse(
                        new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                    return false;
                }

                organisationUser = organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            if (!organisationGatewayResponse.Organisation.UserIsMember(message.OrganisationUserId) ||
                organisationGatewayResponse.Organisation.UserIsOwner(message.OrganisationUserId))
            {
                outputPort.Handle(new DeleteOrganisationUserResponse(
                    new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser userToRemove =
                organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.OrganisationUserId);
            List<Entities.OrganisationUser> users = organisationGatewayResponse.Organisation.Users.ToList();
            users.Remove(userToRemove);
            organisationGatewayResponse.Organisation.Users = users;

            UpdateOrganisationGatewayResponse updateOrganisationGatewayResponse =
                await _organisationRepository.Update(message.OrganisationId, organisationGatewayResponse.Organisation);

            if (!updateOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new DeleteOrganisationUserResponse(updateOrganisationGatewayResponse.Errors,
                        message.UserId != null));
                return false;
            }

            outputPort.Handle(
                new DeleteOrganisationUserResponse(updateOrganisationGatewayResponse.Organisation.ToOrganisation(), organisationUser,
                    message.UserId != null, true));
            return true;
        }
    }
}