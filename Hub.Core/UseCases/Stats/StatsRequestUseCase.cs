using System;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.GatewayResponses.Repositories.Requests;
using Hub.Core.DTO.ResponseMessages.Stats;
using Hub.Core.DTO.UseCaseRequests.Stats;
using Hub.Core.DTO.UseCaseResponses.Stats;
using Hub.Core.Entities;
using Hub.Core.Interfaces.Gateways.Repositories;
using Hub.Core.Interfaces.UseCases.Stats;

namespace Hub.Core.UseCases.Stats
{
    public class StatsRequestUseCase : IStatsRequestUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IRequestsRepository _requestsRepository;

        public StatsRequestUseCase(IClientRepository clientRepository, IRequestsRepository requestsRepository)
        {
            _clientRepository = clientRepository;
            _requestsRepository = requestsRepository;
        }

        public async Task<bool> HandleAsync(StatsRequestRequest message, IOutputPort<StatsRequestResponse> outputPort)
        {
            GetClientGatewayResponse getSendingClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getSendingClientResponse.Success)
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(getSendingClientResponse.Errors)));
                return false;
            }

            GetClientGatewayResponse getRequestedClientResponse = await _clientRepository.Get(message.RequestedClient);

            if (!getRequestedClientResponse.Success)
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(getRequestedClientResponse.Errors)));
                return false;
            }

            if (getSendingClientResponse.Client.OwnerId != getRequestedClientResponse.Client.OwnerId)
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(new[] {new Error("404", "Requested Client doesn't exist or Does not share OwnerId") })));
                return false;
            }
            
            StatsRequest statsRequest = new StatsRequest
            {
                RequestId = Guid.NewGuid(),
                SendingClient = getSendingClientResponse.Client.Id,
                RequestedClient = getRequestedClientResponse.Client.Id
            };

            CreateStatsRequestGatewayResponse createStatsRequestGatewayResponse = await _requestsRepository.CreateStatsRequest(statsRequest);

            if (!createStatsRequestGatewayResponse.Success)
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(createStatsRequestGatewayResponse.Errors)));
                return false;
            }
            
            StatsRequestMessage responseMessage = new StatsRequestMessage(getSendingClientResponse.Client.Id, statsRequest.RequestId, message.StatsSection, true);
            StatsRequestMessage requestMessage = new StatsRequestMessage(getRequestedClientResponse.Client.Id, statsRequest.RequestId, message.StatsSection, true);
            
            outputPort.Handle(new StatsRequestResponse(new[] {requestMessage}, responseMessage));
            return true; 
        }
    }
}