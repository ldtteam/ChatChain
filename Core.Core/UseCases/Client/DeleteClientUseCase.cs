using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
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
    public class DeleteClientUseCase : IDeleteClientUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IClientConfigRepository _clientConfigRepository;
        private readonly IIS4ClientRepository _is4ClientRepository;

        public DeleteClientUseCase(IOrganisationRepository organisationRepository, IClientRepository clientRepository,
            IClientConfigRepository clientConfigRepository, IIS4ClientRepository is4ClientRepository)
        {
            _organisationRepository = organisationRepository;
            _clientRepository = clientRepository;
            _clientConfigRepository = clientConfigRepository;
            _is4ClientRepository = is4ClientRepository;
        }

        public async Task<bool> HandleAsync(DeleteClientRequest message,
            IOutputPort<DeleteClientResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new DeleteClientResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.DeleteClients))
                {
                    outputPort.Handle(new DeleteClientResponse(new[] {new Error("404", "Client Not Found")},
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
                outputPort.Handle(new DeleteClientResponse(getConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (getConfigGatewayResponse.Client.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api clients can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new DeleteClientResponse(new[] {new Error("404", "Client Not Found")},
                    message.UserId != null));
                return false;
            }

            BaseGatewayResponse is4ClientGatewayResponse =
                await _is4ClientRepository.Delete(message.ClientId);

            if (!is4ClientGatewayResponse.Success)
            {
                outputPort.Handle(new DeleteClientResponse(is4ClientGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            BaseGatewayResponse clientGatewayResponse =
                await _clientRepository.Delete(message.ClientId);

            if (!clientGatewayResponse.Success)
            {
                outputPort.Handle(new DeleteClientResponse(clientGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            BaseGatewayResponse clientConfigGatewayResponse =
                await _clientConfigRepository.Delete(message.ClientId);

            if (!clientConfigGatewayResponse.Success)
            {
                outputPort.Handle(new DeleteClientResponse(clientConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new DeleteClientResponse(organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}