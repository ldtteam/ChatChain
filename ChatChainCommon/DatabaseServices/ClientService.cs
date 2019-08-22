using System;
using System.Collections.Generic;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    public class ClientService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<Group> _groups;
        private readonly IServiceProvider _services;

        public ClientService(MongoConnections mongoConnections, IServiceProvider services)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _clients = database.GetCollection<Client>("Clients");
            _groups = database.GetCollection<Group>("Groups");

            _services = services;
        }

        public List<Client> Get()
        {
            return _clients.Find(client => true).ToList();
        }
        
        public List<Client> GetFromOwnerId(string id)
        {
            return _clients.Find(client => client.OwnerId == id).ToList();
        }
        
        public Client Get(ObjectId id)
        {
            return _clients.Find(client => client.Id == id).FirstOrDefault();
        }
        
        public Client Get(string clientId)
        {
            return _clients.Find(client => client.ClientId == clientId).FirstOrDefault();
        }

        public void Create(Client client)
        {
            _clients.InsertOne(client);
        }

        public void Update(ObjectId id, Client clientIn)
        {
            _clients.ReplaceOne(client => client.Id == id, clientIn);
        }

        public void Remove(Client clientIn)
        {
            _clients.DeleteOne(client => client.Id == clientIn.Id);
        }

        public ClientConfig GetClientConfig(ObjectId id)
        {
            Client client = Get(id);
            return _services.GetRequiredService<ClientConfigService>().Get(client.ClientConfigId);
        }
        
        public List<Group> GetGroups(ObjectId id)
        {
            return _groups.Find(group => group.ClientIds.Contains(id)).ToList();
        }
        
        public void AddGroup(ObjectId clientId, ObjectId groupId, bool addClientToGroup = true)
        {

            Client client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            Group group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            List<ObjectId> groupIds = new List<ObjectId>(client.GroupIds) {group.Id};
            client.GroupIds = groupIds;
            Update(client.Id, client);

            if (!addClientToGroup) return;

            _services.GetRequiredService<GroupService>().AddClient(group.Id, client.Id, false);
        }
        
        public void RemoveGroup(ObjectId clientId, ObjectId groupId, bool removeClientFromGroup = true)
        {
            Client client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            Group group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            client.GroupIds.Remove(group.Id);
            Update(client.Id, client);

            if (!removeClientFromGroup) return;

            _services.GetRequiredService<GroupService>().RemoveClient(group.Id, client.Id, false);
        }
    }
}