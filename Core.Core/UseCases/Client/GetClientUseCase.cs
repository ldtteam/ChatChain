using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Client;

namespace Api.Core.UseCases.Client
{
    public class GetClientUseCase : IGetClientUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientRepository _clientRepository;

        public GetClientUseCase(IOrganisationRepository organisationRepository,
            IClientRepository clientRepository)
        {
            _organisationRepository = organisationRepository;
            _clientRepository = clientRepository;
        }

        public async Task<bool> HandleAsync(GetClientRequest message,
            IOutputPort<GetClientResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetClientResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetClientResponse(new[] {new Error("404", "Client Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetClientGatewayResponse clientGatewayResponse =
                await _clientRepository.Get(message.ClientId);

            if (!clientGatewayResponse.Success)
            {
                outputPort.Handle(new GetClientResponse(clientGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (clientGatewayResponse.Client.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api client's can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new GetClientResponse(new[] {new Error("404", "Client Not Found")},
                    message.UserId != null));
                return false;
            }

            outputPort.Handle(new GetClientResponse(clientGatewayResponse.Client,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}