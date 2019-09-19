using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.IS4Client;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IIS4ClientRepository
    {
        Task<CreateIS4ClientGatewayResponse> Create(IS4Client is4Client, string password);
        
        Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId);

        Task<BaseGatewayResponse> Delete(Guid id);
    }
}