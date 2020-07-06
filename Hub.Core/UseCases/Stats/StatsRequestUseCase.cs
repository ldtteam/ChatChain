using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
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
        private readonly IGroupRepository _groupRepository;
        private readonly IRequestsRepository _requestsRepository;

        public StatsRequestUseCase(IClientRepository clientRepository, IGroupRepository groupRepository, IRequestsRepository requestsRepository)
        {
            _clientRepository = clientRepository;
            _groupRepository = groupRepository;
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

            List<Guid> clientIds = new List<Guid>();

            if (message.RequestedClient != Guid.Empty)
            {
                GetClientGatewayResponse getRequestedClientResponse =
                    await _clientRepository.Get(message.RequestedClient);

                if (!getRequestedClientResponse.Success)
                {
                    outputPort.Handle(
                        new StatsRequestResponse(new StatsRequestMessage(getRequestedClientResponse.Errors)));
                    return false;
                }
                
                if (getSendingClientResponse.Client.OwnerId != getRequestedClientResponse.Client.OwnerId)
                {
                    outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(new[] {new Error("404", "Requested Client doesn't exist or Does not share OwnerId") })));
                    return false;
                }
                
                clientIds.Add(getRequestedClientResponse.Client.Id);
            }
            else if (message.RequestedGroup != Guid.Empty)
            {
                GetGroupGatewayResponse getRequestedGroupResponse = await _groupRepository.Get(message.RequestedGroup);

                if (!getRequestedGroupResponse.Success)
                {
                    outputPort.Handle(
                        new StatsRequestResponse(new StatsRequestMessage(getRequestedGroupResponse.Errors)));
                    return false;
                }
                
                if (getSendingClientResponse.Client.OwnerId != getRequestedGroupResponse.Group.OwnerId)
                {
                    outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(new[] {new Error("404", "Requested Group doesn't exist or Does not share OwnerId") })));
                    return false;
                }
                
                clientIds.AddRange(getRequestedGroupResponse.Group.ClientIds);
            }
            else
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(new[] {new Error("400", "RequestedClient or RequestedGroup must be specified") })));
                return false;
            }

            IList<Guid> requestIds = new List<Guid>();
            IList<StatsRequestMessage> requestMessages = new List<StatsRequestMessage>();
            IList<StatsRequest> statsRequests = new List<StatsRequest>();
            
            foreach (Guid clientId in clientIds)
            {
                if (clientId.Equals(message.ClientId))
                    continue;

                Guid requestId = Guid.NewGuid();
                requestIds.Add(requestId);
                StatsRequest statsRequest = new StatsRequest
                {
                    RequestId = requestId,
                    SendingClient = getSendingClientResponse.Client.Id,
                    RequestedClient = clientId
                };

                statsRequests.Add(statsRequest);

                requestMessages.Add(new StatsRequestMessage(clientId, requestId, message.StatsSection, true));
            }
            
            CreateStatsRequestsGatewayResponse createStatsRequestGatewayResponse = await _requestsRepository.CreateStatsRequests(statsRequests);

            if (!createStatsRequestGatewayResponse.Success)
            {
                outputPort.Handle(new StatsRequestResponse(new StatsRequestMessage(new[] {new Error("500", "Failed to create stats requests") })));
                return false;
            }

            StatsRequestMessage responseMessage = new StatsRequestMessage(getSendingClientResponse.Client.Id, requestIds, message.StatsSection, true);
            
            outputPort.Handle(new StatsRequestResponse(requestMessages, responseMessage));
            return true; 
        }
    }
}