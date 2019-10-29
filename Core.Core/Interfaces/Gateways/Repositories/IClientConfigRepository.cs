using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IClientConfigRepository
    {
        Task<GetClientConfigGatewayResponse> Get(Guid id);

        Task<CreateClientConfigGatewayResponse> Create(ClientConfig clientConfig);

        Task<UpdateClientConfigGatewayResponse> Update(Guid id, ClientConfig clientConfig);

        Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId);

        Task<BaseGatewayResponse> Delete(Guid id);
    }
}