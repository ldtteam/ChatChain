using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;

namespace ChatChainCommon.DatabaseServices
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAsync();
        Task<IEnumerable<Client>> GetFromOwnerIdAsync(Guid ownerId);
        Task<Client> GetAsync(Guid clientId);
        Task UpdateAsync(Guid clientId, Client clientIn);
        Task RemoveAsync(Guid clientId);
        Task RemoveForOwnerIdAsync(Guid orgId);
        Task CreateAsync(Client client);
    }
}