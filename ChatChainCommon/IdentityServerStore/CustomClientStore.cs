using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.IdentityServerRepository;
using IdentityServer4.Models;

namespace ChatChainCommon.IdentityServerStore
{
    public class CustomClientStore : IdentityServer4.Stores.IClientStore
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
        
        public Task<List<Client>> AllClients()
        {
            List<Client> clients = _dbRepository.All<Client>().ToList();

            return Task.FromResult(clients);
        }

        public void RemoveClient(Client client)
        {
            _dbRepository.Delete<Client>(c => c.ClientId == client.ClientId);
        }

        public void AddClient(Client client)
        {
            _dbRepository.Add(client);
        }

        public void UpdateClient(Client client)
        {
            _dbRepository.Update(c => c.ClientId == client.ClientId, client);
        }
    }
}