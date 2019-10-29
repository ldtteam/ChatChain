using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Organisation;

namespace Api.Core.UseCases.Organisation
{
    public class DeleteOrganisationUseCase : IDeleteOrganisationUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientConfigRepository _clientConfigRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IIS4ClientRepository _is4ClientRepository;
        private readonly IGroupRepository _groupRepository;

        public DeleteOrganisationUseCase(IOrganisationRepository organisationRepository, IClientConfigRepository clientConfigRepository, IClientRepository clientRepository, IIS4ClientRepository is4ClientRepository, IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _clientConfigRepository = clientConfigRepository;
            _clientRepository = clientRepository;
            _is4ClientRepository = is4ClientRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(DeleteOrganisationRequest message,
            IOutputPort<DeleteOrganisationResponse> outputPort)
        {
            GetOrganisationGatewayResponse getOrganisationGatewayResponse =
                await _organisationRepository.Get(message.Id);

            if (!getOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new DeleteOrganisationResponse(getOrganisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (message.UserId != null && !getOrganisationGatewayResponse.Organisation.UserIsOwner(message.UserId))
            {
                outputPort.Handle(new DeleteOrganisationResponse(new[] {new Error("404", "Organisation Not Found")},
                    message.UserId != null));
                return false;
            }

            BaseGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Delete(getOrganisationGatewayResponse.Organisation.Id);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(new DeleteOrganisationResponse(organisationGatewayResponse.Errors,
                    message.UserId != null));
                return false;
            }

            //Instead of returning when one fails, we run them all and then return all errors together. 
            //This allows the organisation to be cleaned up as much as possible
            bool failure = false;

            BaseGatewayResponse clientConfigGatewayResponse =
                await _clientConfigRepository.DeleteForOwner(getOrganisationGatewayResponse.Organisation.Id);

            if (!clientConfigGatewayResponse.Success) failure = true;

            BaseGatewayResponse is4ClientGatewayResponse =
                await _is4ClientRepository.DeleteForOwner(getOrganisationGatewayResponse.Organisation.Id);
            
            if (!is4ClientGatewayResponse.Success) failure = true;
            
            BaseGatewayResponse clientGatewayResponse =
                await _clientRepository.DeleteForOwner(getOrganisationGatewayResponse.Organisation.Id);

            if (!clientGatewayResponse.Success) failure = true;

            BaseGatewayResponse groupGatewayResponse =
                await _groupRepository.DeleteForOwner(getOrganisationGatewayResponse.Organisation.Id);

            if (!groupGatewayResponse.Success) failure = true;

            if (failure)
            {
                List<Error> errors = new List<Error>();
                errors.AddRange(clientConfigGatewayResponse.Errors);
                errors.AddRange(is4ClientGatewayResponse.Errors);
                errors.AddRange(clientGatewayResponse.Errors);
                errors.AddRange(groupGatewayResponse.Errors);
                outputPort.Handle(new DeleteOrganisationResponse(errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new DeleteOrganisationResponse(message.UserId != null, true));
            return true;
        }
    }
}