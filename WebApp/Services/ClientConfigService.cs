using System;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace WebApp.Services
{
    public class ClientConfigService
    {
        private readonly IMongoCollection<ClientConfig> _clientConfigs;
        private readonly IServiceProvider _services;

        public ClientConfigService(IConfiguration config, IServiceProvider services)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("CLIENTS_AND_GROUPS_DATABASE");

            MongoClient client;
            
            if (databaseUrl != null && !databaseUrl.IsNullOrEmpty())
            {
                client = new MongoClient(databaseUrl);
            }
            else
            {
                client = new MongoClient(config.GetConnectionString("MongoDB"));
            }
            
            var database = client.GetDatabase("ChatChainGroups");
            _clientConfigs = database.GetCollection<ClientConfig>("ClientConfigs");

            _services = services;
        }
        
        public ClientConfig Get(ObjectId id)
        {
            return _clientConfigs.Find(config => config.Id == id).FirstOrDefault();
        }

        public void Create(ClientConfig clientConfig)
        {
            var objectId = new ObjectId();
            clientConfig.Id = objectId;
            _clientConfigs.InsertOne(clientConfig);
            Console.WriteLine("Config Creation: " + Get(objectId).Id);
            Console.WriteLine("Config Creation: " + Get(objectId).ClientId);
            
            
            var clientToUpdate = _services.GetRequiredService<ClientService>().Get(clientConfig.ClientId);
            clientToUpdate.ClientConfigId = objectId;
            _services.GetRequiredService<ClientService>().Update(clientToUpdate.Id, clientToUpdate);
            Console.WriteLine("Config Creation Client: " + _services.GetRequiredService<ClientService>().Get(clientConfig.ClientId).Id);
            Console.WriteLine("Config Creation Client: " + _services.GetRequiredService<ClientService>().Get(clientConfig.ClientId).ClientId);
            Console.WriteLine("Config Creation Client: " + _services.GetRequiredService<ClientService>().Get(clientConfig.ClientId).ClientConfigId);
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
            var config = Get(id);
            return _services.GetRequiredService<ClientService>().Get(config.ClientId);
        }
    }
}