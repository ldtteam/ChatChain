using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.IdentityServerRepository;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace ChatChainCommon.IdentityServerStore
{
    public class CustomPersistedGrantStore: IPersistedGrantStore
    {
        private readonly IRepository _dbRepository;

        public CustomPersistedGrantStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            IQueryable<PersistedGrant> result = _dbRepository.Where<PersistedGrant>(i => i.SubjectId == subjectId);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            PersistedGrant result = _dbRepository.Single<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(result);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);
            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _dbRepository.Add(grant);
            return Task.FromResult(0);
        }
        
    }
}