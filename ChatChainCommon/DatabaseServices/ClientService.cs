using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ClientService : IClientService
    {
        private readonly IMongoCollection<Client> _clients;

        public ClientService(MongoConnections mongoConnections)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _clients = database.GetCollection<Client>("Clients");
        }

        public async Task<IEnumerable<Client>> GetAsync()
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => true);
            return await cursor.ToListAsync();
        }
        
        public async Task<IEnumerable<Client>> GetFromOwnerIdAsync(Guid ownerId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.OwnerId == ownerId);
            return await cursor.ToListAsync();
        }

        public async Task<Client> GetAsync(Guid clientId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.Id == clientId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid clientId, Client clientIn)
        {
            await _clients.ReplaceOneAsync(client => client.Id == clientId, clientIn);
        }

        public async Task RemoveAsync(Guid clientId)
        {
            await _clients.DeleteOneAsync(client => client.Id == clientId);
        }

        public async Task RemoveForOwnerIdAsync(Guid orgId)
        {
            await _clients.DeleteManyAsync(client => client.OwnerId == orgId);
        }
        
        public async Task CreateAsync(Client client)
        {
            await _clients.InsertOneAsync(client);
        }
    }
}