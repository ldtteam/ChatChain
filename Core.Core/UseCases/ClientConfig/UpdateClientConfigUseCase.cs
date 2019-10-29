using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.ClientConfig;
using Api.Core.DTO.UseCaseResponses.ClientConfig;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.ClientConfig;

namespace Api.Core.UseCases.ClientConfig
{
    public sealed class UpdateClientConfigUseCase : IUpdateClientConfigUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientConfigRepository _clientConfigRepository;

        public UpdateClientConfigUseCase(IOrganisationRepository organisationRepository,
            IClientConfigRepository clientConfigRepository)
        {
            _organisationRepository = organisationRepository;
            _clientConfigRepository = clientConfigRepository;
        }

        public async Task<bool> HandleAsync(UpdateClientConfigRequest message,
            IOutputPort<UpdateClientConfigResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateClientConfigResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.EditClients))
                {
                    outputPort.Handle(new UpdateClientConfigResponse(
                        new[] {new Error("404", "Client Config Not Found")}, message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetClientConfigGatewayResponse getConfigGatewayResponse =
                await _clientConfigRepository.Get(message.ClientConfigId);

            if (!getConfigGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateClientConfigResponse(getConfigGatewayResponse.Errors,
                    message.UserId != null));
                return false;
            }

            if (getConfigGatewayResponse.ClientConfig.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api clients can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new UpdateClientConfigResponse(new[] {new Error("404", "Client Config Not Found")},
                    message.UserId != null));
                return false;
            }

            getConfigGatewayResponse.ClientConfig.ClientEventGroups = message.ClientEventGroups;
            getConfigGatewayResponse.ClientConfig.UserEventGroups = message.UserEventGroups;

            UpdateClientConfigGatewayResponse configGatewayResponse =
                await _clientConfigRepository.Update(getConfigGatewayResponse.ClientConfig.Id,
                    getConfigGatewayResponse.ClientConfig);

            if (!configGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateClientConfigResponse(configGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new UpdateClientConfigResponse(configGatewayResponse.ClientConfig,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}