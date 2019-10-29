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
    public class UpdateOrganisationUserUseCase : IUpdateOrganisationUserUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationUserUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(UpdateOrganisationUserRequest message,
            IOutputPort<UpdateOrganisationUserResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateOrganisationUserResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.EditOrgUsers))
                {
                    outputPort.Handle(new UpdateOrganisationUserResponse(
                        new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                    return false;
                }

                organisationUser = organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            if (!organisationGatewayResponse.Organisation.UserIsMember(message.OrganisationUserId) ||
                organisationGatewayResponse.Organisation.UserIsOwner(message.OrganisationUserId))
            {
                outputPort.Handle(new UpdateOrganisationUserResponse(
                    new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser userToUpdate =
                organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.OrganisationUserId);
            userToUpdate.Permissions = message.Permissions;

            UpdateOrganisationGatewayResponse updateOrganisationGatewayResponse =
                await _organisationRepository.Update(message.OrganisationId, organisationGatewayResponse.Organisation);

            if (!updateOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateOrganisationUserResponse(updateOrganisationGatewayResponse.Errors,
                        message.UserId != null));
                return false;
            }

            outputPort.Handle(
                new UpdateOrganisationUserResponse(userToUpdate, updateOrganisationGatewayResponse.Organisation.ToOrganisation(),
                    organisationUser, message.UserId != null, true));
            return true;
        }
    }
}