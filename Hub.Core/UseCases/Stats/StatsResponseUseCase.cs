using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.GatewayResponses.Repositories.Requests;
using Hub.Core.DTO.ResponseMessages.Stats;
using Hub.Core.DTO.UseCaseRequests.Stats;
using Hub.Core.DTO.UseCaseResponses.Stats;
using Hub.Core.Interfaces.Gateways.Repositories;
using Hub.Core.Interfaces.UseCases.Stats;

namespace Hub.Core.UseCases.Stats
{
    public class StatsResponseUseCase : IStatsResponseUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IRequestsRepository _requestsRepository;

        public StatsResponseUseCase(IClientRepository clientRepository, IRequestsRepository requestsRepository)
        {
            _clientRepository = clientRepository;
            _requestsRepository = requestsRepository;
        }
        
        public async Task<bool> HandleAsync(StatsResponseRequest message, IOutputPort<StatsResponseResponse> outputPort)
        {
            GetClientGatewayResponse getSendingClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getSendingClientResponse.Success)
            {
                outputPort.Handle(new StatsResponseResponse(new StatsResponseMessage(getSendingClientResponse.Errors)));
                return false;
            }

            GetStatsRequestGatewayResponse getStatsRequestGatewayResponse =
                await _requestsRepository.GetStatsRequest(message.RequestId);

            if (!getStatsRequestGatewayResponse.Success)
            {
                outputPort.Handle(new StatsResponseResponse(new StatsResponseMessage(getStatsRequestGatewayResponse.Errors)));
                return false;
            }

            if (!getStatsRequestGatewayResponse.StatsRequest.RequestedClient.Equals(getSendingClientResponse.Client.Id))
            {
                outputPort.Handle(new StatsResponseResponse(new StatsResponseMessage(new[] {new Error("404", "Stats Request doesn't exist or wasn't for this client")})));
                return false;
            }
            
            GetClientGatewayResponse getRequestingClientResponse = await _clientRepository.Get(getStatsRequestGatewayResponse.StatsRequest.SendingClient);

            if (!getRequestingClientResponse.Success)
            {
                outputPort.Handle(new StatsResponseResponse(new StatsResponseMessage(getSendingClientResponse.Errors)));
                return false;
            }

            StatsResponseMessage responseMessage = new StatsResponseMessage(getSendingClientResponse.Client, getStatsRequestGatewayResponse.StatsRequest.SendingClient, getStatsRequestGatewayResponse.StatsRequest.RequestId, message.StatsObject);
            StatsResponseMessage requestMessage = new StatsResponseMessage(getSendingClientResponse.Client, message.ClientId, getStatsRequestGatewayResponse.StatsRequest.RequestId, message.StatsObject);
            
            outputPort.Handle(new StatsResponseResponse(new[] {responseMessage}, requestMessage));
            return true;
        }
    }
}