using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseResponses;
using Hub.Core.Interfaces.UseCases;

namespace Hub.Core.UseCases
{
    public class GenericMessageUseCase : IGenericMessageUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IGroupRepository _groupRepository;

        public GenericMessageUseCase(IClientRepository clientRepository, IGroupRepository groupRepository)
        {
            _clientRepository = clientRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(GenericMessageRequest message, IOutputPort<GenericMessageResponse> outputPort)
        {
            GetClientGatewayResponse getClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getClientResponse.Success)
            {
                outputPort.Handle(new GenericMessageResponse(new GenericMessageMessage(getClientResponse.Errors)));
                return false;
            }

            GetGroupGatewayResponse getGroupResponse = await _groupRepository.Get(message.GroupId);

            if (!getGroupResponse.Success)
            {
                outputPort.Handle(new GenericMessageResponse(new GenericMessageMessage(getGroupResponse.Errors)));
                return false;
            }

            if (!getGroupResponse.Group.ClientIds.Contains(getClientResponse.Client.Id))
            {
                outputPort.Handle(new GenericMessageResponse(new GenericMessageMessage(new[] {new Error("403", "Sending Client not in Group specified")})));
                return false;
            }

            IList<Guid> clientIds = getGroupResponse.Group.ClientIds;
            clientIds.Remove(getClientResponse.Client.Id);

            IList<GenericMessageMessage> messages = new List<GenericMessageMessage>();
            foreach (Guid clientId in clientIds)
            {
                GetClientGatewayResponse getGroupClientResponse = await _clientRepository.Get(clientId);

                if (getGroupClientResponse.Success)
                {
                    messages.Add(new GenericMessageMessage(getClientResponse.Client, getGroupClientResponse.Client.Id, getGroupResponse.Group, message.ClientUser, message.Message, true));
                }
            }
            
            GenericMessageMessage responseMessage = new GenericMessageMessage(getClientResponse.Client, getClientResponse.Client.Id, getGroupResponse.Group, message.ClientUser, message.Message, true);

            outputPort.Handle(new GenericMessageResponse(messages, responseMessage));
            return true;
        }
    }
}