using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IGroupRepository
    {
        Task<GetGroupsGatewayResponse> GetForOwner(Guid ownerId);

        Task<GetGroupGatewayResponse> Get(Guid id);

        Task<CreateGroupGatewayResponse> Create(Group group);

        Task<UpdateGroupGatewayResponse> Update(Guid groupId, Group group);

        Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId);

        Task<BaseGatewayResponse> Delete(Guid id);
    }
}