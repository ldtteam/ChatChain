using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseResponses;
using Hub.Core.Interfaces.UseCases;

namespace Hub.Core.UseCases
{
    public class GetGroupsUseCase : IGetGroupsUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IGroupRepository _groupRepository;

        public GetGroupsUseCase(IClientRepository clientRepository, IGroupRepository groupRepository)
        {
            _clientRepository = clientRepository;
            _groupRepository = groupRepository;
        }
        
        public async Task<bool> HandleAsync(GetGroupsRequest message, IOutputPort<GetGroupsResponse> outputPort)
        {
            GetClientGatewayResponse getClientResponse = await _clientRepository.Get(message.ClientId);

            if (!getClientResponse.Success)
            {
                outputPort.Handle(new GetGroupsResponse(new GetGroupsMessage(getClientResponse.Errors)));
                return false;
            }

            GetGroupsGatewayResponse getGroupsResponse =
                await _groupRepository.GetForOwner(getClientResponse.Client.OwnerId);
            
            if (!getGroupsResponse.Success)
            {
                outputPort.Handle(new GetGroupsResponse(new GetGroupsMessage(getGroupsResponse.Errors)));
                return false;
            }

            IList<Group> groups =
                getGroupsResponse.Groups.Where(group => group.ClientIds.Contains(getClientResponse.Client.Id)).ToList();
            
            outputPort.Handle(new GetGroupsResponse(new GetGroupsMessage(groups, true)));
            return true;
        }
    }
}