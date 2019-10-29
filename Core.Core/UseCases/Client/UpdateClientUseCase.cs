using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Client;

namespace Api.Core.UseCases.Client
{
    public class UpdateClientUseCase : IUpdateClientUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientRepository _clientRepository;

        public UpdateClientUseCase(IOrganisationRepository organisationRepository,
            IClientRepository clientRepository)
        {
            _organisationRepository = organisationRepository;
            _clientRepository = clientRepository;
        }

        public async Task<bool> HandleAsync(UpdateClientRequest message, IOutputPort<UpdateClientResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateClientResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.EditClients))
                {
                    outputPort.Handle(new UpdateClientResponse(new[] {new Error("404", "Client Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetClientGatewayResponse getConfigGatewayResponse =
                await _clientRepository.Get(message.ClientId);

            if (!getConfigGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateClientResponse(getConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (getConfigGatewayResponse.Client.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api clients can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new UpdateClientResponse(new[] {new Error("404", "Client Not Found")},
                    message.UserId != null));
                return false;
            }

            getConfigGatewayResponse.Client.Name = message.Name;
            getConfigGatewayResponse.Client.Description = message.Description;

            UpdateClientGatewayResponse clientGatewayResponse =
                await _clientRepository.Update(message.ClientId, getConfigGatewayResponse.Client);

            if (!clientGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateClientResponse(clientGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new UpdateClientResponse(clientGatewayResponse.Client,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}