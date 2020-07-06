using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.ResponseMessages.Events;
using Hub.Core.DTO.UseCaseRequests.Events;
using Hub.Core.DTO.UseCaseResponses.Events;
using Hub.Core.Interfaces.UseCases.Events;

namespace Hub.Core.UseCases.Events
{
    public class UserEventUseCase : IUserEventUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientConfigRepository _clientConfigRepository;
        private readonly IGroupRepository _groupRepository;

        public UserEventUseCase(IClientRepository clientRepository, IClientConfigRepository clientConfigRepository, IGroupRepository groupRepository)
        {
            _clientRepository = clientRepository;
            _clientConfigRepository = clientConfigRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(UserEventRequest message, IOutputPort<UserEventResponse> outputPort)
        {
            GetClientGatewayResponse getClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getClientResponse.Success)
            {
                outputPort.Handle(new UserEventResponse(new UserEventMessage(getClientResponse.Errors)));
                return false;
            }

            GetClientConfigGatewayResponse getClientConfigResponse = await _clientConfigRepository.Get(message.ClientId);

            if (!getClientConfigResponse.Success)
            {
                outputPort.Handle(new UserEventResponse(new UserEventMessage(getClientConfigResponse.Errors)));
                return false;
            }

            IList<UserEventMessage> messages = new List<UserEventMessage>();
            IList<Group> groups = new List<Group>();

            foreach (Guid clientConfigUserEventGroup in getClientConfigResponse.ClientConfig.UserEventGroups)
            {
                GetGroupGatewayResponse getGroupResponse =
                    await _groupRepository.Get(clientConfigUserEventGroup);

                if (!getGroupResponse.Success) continue;
                
                groups.Add(getGroupResponse.Group);

                foreach (Guid groupClientId in getGroupResponse.Group.ClientIds)
                {
                    if (groupClientId.Equals(getClientResponse.Client.Id) && !message.SendToSelf) continue;
                    
                    GetClientGatewayResponse getGroupClientResponse = await _clientRepository.Get(groupClientId);

                    if (!getGroupResponse.Success) continue;
                    
                    messages.Add(new UserEventMessage(getClientResponse.Client, message.ClientUser, getGroupClientResponse.Client.Id, getGroupResponse.Group, message.Event, message.EventData, true));
                }
            }
            
            UserEventMessage responseMessage = new UserEventMessage(getClientResponse.Client, message.ClientUser,getClientResponse.Client.Id, groups, message.Event, message.EventData, true);
            
            outputPort.Handle(new UserEventResponse(messages, responseMessage));
            return true;
        }
    }
}