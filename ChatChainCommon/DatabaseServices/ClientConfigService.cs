using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ClientConfigService : IClientConfigService
    {
        private readonly IMongoCollection<ClientConfig> _clientConfigs;

        public ClientConfigService(MongoConnections mongoConnections)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _clientConfigs = database.GetCollection<ClientConfig>("ClientConfigs");
        }

        public async Task<ClientConfig> GetAsync(Guid id)
        {
            IAsyncCursor<ClientConfig> cursor = await _clientConfigs.FindAsync(config => config.Id == id);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task CreateAsync(ClientConfig clientConfig)
        {
            await _clientConfigs.InsertOneAsync(clientConfig);
        }

        public async Task RemoveAsync(Guid id)
        {
            await _clientConfigs.DeleteOneAsync(config => config.Id == id);
        }
        
        public async Task UpdateAsync(Guid id, ClientConfig clientConfig)
        {
            await _clientConfigs.ReplaceOneAsync(config => config.Id == id, clientConfig);
        }
    }
}