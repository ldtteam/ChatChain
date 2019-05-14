using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainServer.Models;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainServer.Services
{
    public class ClientUserService
    {
        private readonly IMongoCollection<ClientUser> _clientUsers;
        private readonly IServiceProvider _services;

        public ClientUserService(IConfiguration config, IServiceProvider services)
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
            _clientUsers = database.GetCollection<ClientUser>("ClientUsers");

            _services = services;
        }

        public async Task<ClientUser> Get(ObjectId id)
        {
            return (await _clientUsers.FindAsync(user => user.Id == id)).FirstOrDefault();
        }

        public async Task<IEnumerable<ClientUser>> GetFromOwnerId(string id)
        {
            return (await _clientUsers.FindAsync(user => user.OwnerId == id)).ToList();
        }

        public async void Create(ClientUser user)
        {
            await _clientUsers.InsertOneAsync(user);
        }

        public async void Update(ClientUser inputUser)
        {
            await _clientUsers.ReplaceOneAsync(user => user.Id == inputUser.Id, inputUser);
        }

    }
}