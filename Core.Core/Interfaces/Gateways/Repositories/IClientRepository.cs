using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IClientRepository
    {
        Task<GetClientsGatewayResponse> GetForOwner(Guid ownerId);

        Task<GetClientGatewayResponse> Get(Guid id);

        Task<CreateClientGatewayResponse> Create(Client client);

        Task<UpdateClientGatewayResponse> Update(Guid clientId, Client client);

        Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId);

        Task<BaseGatewayResponse> Delete(Guid id);
    }
}