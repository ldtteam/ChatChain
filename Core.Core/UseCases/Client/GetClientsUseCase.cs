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
    public class GetClientsUseCase : IGetClientsUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientRepository _clientRepository;

        public GetClientsUseCase(IOrganisationRepository organisationRepository,
            IClientRepository clientRepository)
        {
            _organisationRepository = organisationRepository;
            _clientRepository = clientRepository;
        }

        public async Task<bool> HandleAsync(GetClientsRequest message,
            IOutputPort<GetClientsResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetClientsResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetClientsResponse(new[] {new Error("404", "Clients Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetClientsGatewayResponse clientsGatewayResponse =
                await _clientRepository.GetForOwner(organisationGatewayResponse.Organisation.Id);

            if (!clientsGatewayResponse.Success)
            {
                outputPort.Handle(new GetClientsResponse(clientsGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new GetClientsResponse(clientsGatewayResponse.Clients,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}