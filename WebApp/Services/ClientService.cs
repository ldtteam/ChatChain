using System;
using System.Collections.Generic;
using IdentityServer4.Extensions;
using IdentityServer_WebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer_WebApp.Services
{
    public class ClientService
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<Group> _groups;
        private readonly IServiceProvider _services;

        public ClientService(IConfiguration config, IServiceProvider services)
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
            _clients = database.GetCollection<Client>("Clients");
            _groups = database.GetCollection<Group>("Groups");

            _services = services;
        }

        public List<Client> Get()
        {
            return _clients.Find(client => true).ToList();
        }

        public Client Get(string id)
        {
            var docId = new ObjectId(id);

            return _clients.Find(client => client.Id == docId).FirstOrDefault();
        }

        public List<Client> GetFromOwnerId(string id)
        {
            return _clients.Find(client => client.OwnerId == id).ToList();
        }

        public Client GetFromClientId(string id)
        {
            return _clients.Find(client => client.ClientId == id).FirstOrDefault();
        }

        public void Create(Client client)
        {
            _clients.InsertOne(client);
        }

        public void Update(string id, Client clientIn)
        {
            var docId = new ObjectId(id);

            _clients.ReplaceOne(client => client.Id == docId, clientIn);
        }

        public void Remove(Client clientIn)
        {
            _clients.DeleteOne(client => client.Id == clientIn.Id);
        }

        public void Remove(string id)
        {
            var docId = new ObjectId(id);
            
            _clients.DeleteOne(client => client.Id == docId);
        }

        public List<Group> GetGroups(string id)
        {
            var docId = new ObjectId(id);

            return _groups.Find(group => group.ClientIds.Contains(docId)).ToList();
        }
        
        public void AddGroup(ObjectId clientId, ObjectId groupId, bool addClientToGroup = true)
        {

            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            var groupIds = new List<ObjectId>(client.GroupIds) {group.Id};
            client.GroupIds = groupIds;
            Update(client.Id.ToString(), client);

            if (!addClientToGroup) return;

            _services.GetRequiredService<GroupService>().AddClient(group.Id, client.Id, false);
        }
        
        public void RemoveGroup(ObjectId clientId, ObjectId groupId, bool removeClientFromGroup = true)
        {

            var client = _clients.Find(lclient => lclient.Id == clientId).FirstOrDefault();
            var group = _groups.Find(lgroup => lgroup.Id == groupId).FirstOrDefault();
            
            if (client == null || group == null) return;
            
            client.GroupIds.Remove(group.Id);
            Update(client.Id.ToString(), client);

            if (!removeClientFromGroup) return;

            _services.GetRequiredService<GroupService>().RemoveClient(group.Id, client.Id, false);
        }
    }
}