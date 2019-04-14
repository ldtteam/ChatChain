using System;
using System.Collections.Generic;
using ChatChainServer.Models;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainServer.Services
{
    public class GroupService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<Group> _groups;
        private readonly IServiceProvider _services;

        public GroupService(IConfiguration config, IServiceProvider services)
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
            _groups = database.GetCollection<Group>("Groups");
            _clients = database.GetCollection<Client>("Clients");

            _services = services;
        }

        public List<Group> Get()
        {
            return _groups.Find(group => true).ToList();
        }

        public Group Get(ObjectId id)
        {
            return _groups.Find(group => group.Id == id).FirstOrDefault();
        }
        
        public Group GetFromGuid(string id)
        {
            return _groups.Find(group => group.GroupId == id).FirstOrDefault();
        }

        public void Create(Group group)
        {
            _groups.InsertOne(group);
        }

        public void Update(ObjectId id, Group groupIn)
        {
            _groups.ReplaceOne(group => group.Id == id, groupIn);
        }

        public void Remove(Group groupIn)
        {
            _groups.DeleteOne(group => group.Id == groupIn.Id);
        }

        public void Remove(ObjectId id)
        {
            _groups.DeleteOne(group => group.Id == id);
        }

        public List<Client> GetClients(ObjectId id)
        {
            return _clients.Find(client => client.GroupIds.Contains(id)).ToList();
        }

        public void AddClient(ObjectId groupId, ObjectId clientId, bool addGroupToClient = true)
        {

            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            
            if (group == null || client == null) return;

            var clientIds = new List<ObjectId>(group.ClientIds) {client.Id};
            group.ClientIds = clientIds;
            Update(group.Id, group);
            
            if (!addGroupToClient) return;

            _services.GetRequiredService<ClientService>().AddGroup(client.Id, group.Id, false);
        }
        
        public void RemoveClient(ObjectId groupId, ObjectId clientId, bool removeGroupFromClient = true)
        {

            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            
            if (group == null || client == null) return;
            
            group.ClientIds.Remove(client.Id);
            Update(group.Id, group);
            
            if (!removeGroupFromClient) return;

            _services.GetRequiredService<ClientService>().RemoveGroup(client.Id, group.Id, false);
        }
    }
}