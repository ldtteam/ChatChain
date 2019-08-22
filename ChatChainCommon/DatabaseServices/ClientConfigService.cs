using System;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
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
        
        public ClientConfig Get(ObjectId id)
        {
            return _clientConfigs.Find(config => config.Id == id).FirstOrDefault();
        }

        public ClientConfig GetForClientId(ObjectId id)
        {
            return _clientConfigs.Find(config => config.ClientId == id).FirstOrDefault();
        }

        public void Create(ClientConfig clientConfig)
        {
            _clientConfigs.InsertOne(clientConfig);
            ObjectId clientId = clientConfig.ClientId;
            
            Client clientToUpdate = _services.GetRequiredService<ClientService>().Get(clientConfig.ClientId);
            clientToUpdate.ClientConfigId = GetForClientId(clientId).Id;
            _services.GetRequiredService<ClientService>().Update(clientToUpdate.Id, clientToUpdate);
        }

        public void Remove(ObjectId id)
        {
            _clientConfigs.DeleteOne(config => config.Id == id);
        }
        
        public void Update(ObjectId id, ClientConfig clientConfig)
        {
            _clientConfigs.ReplaceOne(config => config.Id == id, clientConfig);
        }

        public Client GetClient(ObjectId id)
        {
            ClientConfig config = Get(id);
            return _services.GetRequiredService<ClientService>().Get(config.ClientId);
        }
    }
}