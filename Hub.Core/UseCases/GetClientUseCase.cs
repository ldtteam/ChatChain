using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseResponses;
using Hub.Core.Interfaces.UseCases;

namespace Hub.Core.UseCases
{
    public class GetClientUseCase : IGetClientUseCase
    {
        private readonly IClientRepository _clientRepository;

        public GetClientUseCase(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        public async Task<bool> HandleAsync(GetClientRequest message, IOutputPort<GetClientResponse> outputPort)
        {
            GetClientGatewayResponse getClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getClientResponse.Success)
            {
                outputPort.Handle(new GetClientResponse(new GetClientMessage(getClientResponse.Errors)));
                return false;
            }

            outputPort.Handle(new GetClientResponse(new GetClientMessage(getClientResponse.Client, true)));
            return true;
        }
    }
}