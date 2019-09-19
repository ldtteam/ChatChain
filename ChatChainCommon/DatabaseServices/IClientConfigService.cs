using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;

namespace ChatChainCommon.DatabaseServices
{
    public interface IClientConfigService
    {
        Task<ClientConfig> GetAsync(Guid id);
        Task<IEnumerable<ClientConfig>> GetFromOwnerIdAsync(Guid ownerId);
        Task CreateAsync(ClientConfig clientConfig);
        Task RemoveAsync(Guid id);
        Task RemoveForOwnerIdAsync(Guid orgId);
        Task UpdateAsync(Guid id, ClientConfig clientConfig);
    }
}