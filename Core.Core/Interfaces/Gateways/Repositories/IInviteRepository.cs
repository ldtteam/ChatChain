using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IInviteRepository
    {
        Task<GetInviteGatewayResponse> Get(Guid organisationId, string token);

        Task<CreateInviteGatewayResponse> Create(Invite invite);

        Task<BaseGatewayResponse> Delete(Guid organisationId, string token);
    }
}