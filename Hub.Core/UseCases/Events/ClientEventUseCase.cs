using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.ResponseMessages.Events;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseRequests.Events;
using Hub.Core.DTO.UseCaseResponses;
using Hub.Core.DTO.UseCaseResponses.Events;
using Hub.Core.Interfaces.UseCases.Events;

namespace Hub.Core.UseCases.Events
{
    public class ClientEventUseCase : IClientEventUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientConfigRepository _clientConfigRepository;
        private readonly IGroupRepository _groupRepository;

        public ClientEventUseCase(IClientRepository clientRepository, IClientConfigRepository clientConfigRepository, IGroupRepository groupRepository)
        {
            _clientRepository = clientRepository;
            _clientConfigRepository = clientConfigRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(ClientEventRequest message, IOutputPort<ClientEventResponse> outputPort)
        {
            GetClientGatewayResponse getClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getClientResponse.Success)
            {
                outputPort.Handle(new ClientEventResponse(new ClientEventMessage(getClientResponse.Errors)));
                return false;
            }

            GetClientConfigGatewayResponse getClientConfigResponse = await _clientConfigRepository.Get(message.ClientId);

            if (!getClientConfigResponse.Success)
            {
                outputPort.Handle(new ClientEventResponse(new ClientEventMessage(getClientConfigResponse.Errors)));
                return false;
            }

            IList<ClientEventMessage> messages = new List<ClientEventMessage>();
            IList<Group> groups = new List<Group>();

            foreach (Guid clientConfigClientEventGroup in getClientConfigResponse.ClientConfig.ClientEventGroups)
            {
                GetGroupGatewayResponse getGroupResponse =
                    await _groupRepository.Get(clientConfigClientEventGroup);

                if (!getGroupResponse.Success) continue;
                
                groups.Add(getGroupResponse.Group);

                foreach (Guid groupClientId in getGroupResponse.Group.ClientIds)
                {
                    if (groupClientId.Equals(getClientResponse.Client.Id) && !message.SendToSelf) continue;
                    
                    GetClientGatewayResponse getGroupClientResponse = await _clientRepository.Get(groupClientId);

                    if (!getGroupResponse.Success) continue;
                    
                    messages.Add(new ClientEventMessage(getClientResponse.Client, getGroupClientResponse.Client.Id, getGroupResponse.Group, message.Event, message.EventData, true));
                }
            }
            
            ClientEventMessage responseMessage = new ClientEventMessage(getClientResponse.Client, getClientResponse.Client.Id, groups, message.Event, message.EventData, true);
            
            outputPort.Handle(new ClientEventResponse(messages, responseMessage));
            return true;
        }
    }
}