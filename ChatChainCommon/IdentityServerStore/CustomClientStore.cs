using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.IdentityServerRepository;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace ChatChainCommon.IdentityServerStore
{
    public class CustomClientStore : ICustomClientStore, IClientStore
    {
        private readonly IRepository _dbRepository;
        private readonly IEnumerable<Client> _clients;

        public CustomClientStore(IRepository repository, IEnumerable<Client> clients = null)
        {
            _dbRepository = repository;
            _clients = clients;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            IEnumerable<Client> query =
                from lClient in _clients
                where lClient.ClientId == clientId
                select lClient;
            
            Client client = query.SingleOrDefault() ?? _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }

        public void RemoveClient(string clientId)
        {
            _dbRepository.Delete<Client>(c => c.ClientId == clientId);
        }

        public void AddClient(Client client)
        {
            _dbRepository.Add(client);
        }
    }
}