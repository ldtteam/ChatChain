using System;
using System.Collections.Generic;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    public class GroupService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<Group> _groups;
        private readonly IServiceProvider _services;

        public GroupService(MongoConnections mongoConnections, IServiceProvider services)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _groups = database.GetCollection<Group>("Groups");
            _clients = database.GetCollection<Client>("Clients");

            _services = services;
        }

        public List<Group> Get()
        {
            return _groups.Find(group => true).ToList();
        }

        public Group Get(string id)
        {
            ObjectId docId = new ObjectId(id);

            return _groups.Find(group => group.Id == docId).FirstOrDefault();
        }

        public void Create(Group group)
        {
            _groups.InsertOne(group);
        }

        public void Update(string id, Group groupIn)
        {
            ObjectId docId = new ObjectId(id);

            _groups.ReplaceOne(group => group.Id == docId, groupIn);

            //_groups.ReplaceOne(group => group.Id == docId, groupIn);
        }

        public void Remove(Group groupIn)
        {
            _groups.DeleteOne(group => group.Id == groupIn.Id);
        }

        public void Remove(string id)
        {
            ObjectId docId = new ObjectId(id);
            
            _groups.DeleteOne(group => group.Id == docId);
        }

        public List<Client> GetClients(string groupId)
        {
            ObjectId docId = new ObjectId(groupId);

            return _clients.Find(client => client.GroupIds.Contains(docId)).ToList();
        }

        public void AddClient(ObjectId groupId, ObjectId clientId, bool addGroupToClient = true)
        {
            Group group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            Client client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            
            if (group == null || client == null) return;

            List<ObjectId> clientIds = new List<ObjectId>(group.ClientIds) {client.Id};
            group.ClientIds = clientIds;
            Update(group.Id.ToString(), group);
            
            if (!addGroupToClient) return;

            _services.GetRequiredService<ClientService>().AddGroup(client.Id, group.Id, false);
        }
        
        public void RemoveClient(ObjectId groupId, ObjectId clientId, bool removeGroupFromClient = true)
        {
            Group group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            Client client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            
            if (group == null || client == null) return;
            
            group.ClientIds.Remove(client.Id);
            Update(group.Id.ToString(), group);
            
            if (!removeGroupFromClient) return;

            _services.GetRequiredService<ClientService>().RemoveGroup(client.Id, group.Id, false);
        }
    }
}