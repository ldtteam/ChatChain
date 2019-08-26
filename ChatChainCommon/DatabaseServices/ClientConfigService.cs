using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ClientConfigService
    {
        private readonly IMongoCollection<ClientConfig> _clientConfigs;
        private readonly IServiceProvider _services;

        public ClientConfigService(MongoConnections mongoConnections, IServiceProvider services)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _clientConfigs = database.GetCollection<ClientConfig>("ClientConfigs");

            _services = services;
        }

        public async Task<ClientConfig> GetAsync(ObjectId id)
        {
            IAsyncCursor<ClientConfig> cursor = await _clientConfigs.FindAsync(config => config.Id == id);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<ClientConfig> GetForClientIdAsync(ObjectId id)
        {
            IAsyncCursor<ClientConfig> cursor = await _clientConfigs.FindAsync(config => config.ClientId == id);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ClientConfig clientConfig)
        {
            await _clientConfigs.InsertOneAsync(clientConfig);
            ObjectId clientId = clientConfig.ClientId;

            ClientService clientService = _services.GetRequiredService<ClientService>();
            
            Client clientToUpdate = await clientService.GetAsync(clientConfig.ClientId);
            clientToUpdate.ClientConfigId = (await GetForClientIdAsync(clientId)).Id;
            await clientService.UpdateAsync(clientToUpdate.Id, clientToUpdate);
        }

        public async Task RemoveAsync(ObjectId id)
        {
            await _clientConfigs.DeleteOneAsync(config => config.Id == id);
        }
        
        public async Task UpdateAsync(ObjectId id, ClientConfig clientConfig)
        {
            await _clientConfigs.ReplaceOneAsync(config => config.Id == id, clientConfig);
        }

        public async Task<Client> GetClientAsync(ObjectId id)
        {
            ClientConfig config = await GetAsync(id);
            return await _services.GetRequiredService<ClientService>().GetAsync(config.ClientId);
        }
    }
}