using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.ClientConfig;
using Api.Core.DTO.UseCaseResponses.ClientConfig;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.ClientConfig;

namespace Api.Core.UseCases.ClientConfig
{
    public class GetClientConfigUseCase : IGetClientConfigUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientConfigRepository _clientConfigRepository;

        public GetClientConfigUseCase(IOrganisationRepository organisationRepository,
            IClientConfigRepository clientConfigRepository)
        {
            _organisationRepository = organisationRepository;
            _clientConfigRepository = clientConfigRepository;
        }

        public async Task<bool> HandleAsync(GetClientConfigRequest message,
            IOutputPort<GetClientConfigResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetClientConfigResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetClientConfigResponse(new[] {new Error("404", "Client Config Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            DTO.GatewayResponses.Repositories.ClientConfig.GetClientConfigGatewayResponse configGatewayResponse =
                await _clientConfigRepository.Get(message.ClientConfigId);

            if (!configGatewayResponse.Success)
            {
                outputPort.Handle(new GetClientConfigResponse(configGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (configGatewayResponse.ClientConfig.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api client's can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new GetClientConfigResponse(new[] {new Error("404", "Client Config Not Found")},
                    message.UserId != null));
                return false;
            }

            outputPort.Handle(new GetClientConfigResponse(configGatewayResponse.ClientConfig,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}