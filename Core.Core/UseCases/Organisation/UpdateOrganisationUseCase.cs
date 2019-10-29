using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Organisation;

namespace Api.Core.UseCases.Organisation
{
    public class UpdateOrganisationUseCase : IUpdateOrganisationUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(UpdateOrganisationRequest message,
            IOutputPort<UpdateOrganisationResponse> outputPort)
        {
            GetOrganisationGatewayResponse getOrganisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!getOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateOrganisationResponse(getOrganisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!getOrganisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.EditOrg))
                {
                    outputPort.Handle(new UpdateOrganisationResponse(new[] {new Error("404", "Organisation Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    getOrganisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            getOrganisationGatewayResponse.Organisation.Name = message.Name;

            UpdateOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Update(getOrganisationGatewayResponse.Organisation.Id,
                    getOrganisationGatewayResponse.Organisation);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateOrganisationResponse(organisationGatewayResponse.Errors,
                    message.UserId != null));
                return false;
            }

            outputPort.Handle(new UpdateOrganisationResponse(organisationGatewayResponse.Organisation,
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}