using System;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;

namespace ChatChainCommon.DatabaseServices
{
    public interface IClientConfigService
    {
        Task<ClientConfig> GetAsync(Guid id);
        Task CreateAsync(ClientConfig clientConfig);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Guid id, ClientConfig clientConfig);
    }
}