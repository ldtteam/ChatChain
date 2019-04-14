using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using WebApp.Repository;

namespace IdentityServer.Store
{
    public class CustomClientStore : IdentityServer4.Stores.IClientStore
    {
        private readonly IRepository _dbRepository;

        public CustomClientStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }

        public Task<List<Client>> AllClients()
        {
            var clients = _dbRepository.All<Client>().ToList();

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