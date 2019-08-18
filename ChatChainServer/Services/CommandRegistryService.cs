using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainServer.Models.CommandObjects;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ChatChainServer.Services
{
    public class CommandRegistryService
    {
        private readonly IMongoCollection<Command> _commands;
        
        public CommandRegistryService(IConfiguration config)
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
            _commands = database.GetCollection<Command>("Commands");
        }

        public Command Get(string id)
        {
            return _commands.Find(command => command.Id.Equals(id)).FirstOrDefault();
        }

        public IEnumerable<Command> GetFromOwnerId(string id)
        {
            return _commands.Find(command => command.OwnerId == id).ToList();
        }

        public IEnumerable<Command> GetFromClientGuid(string id)
        {
            return _commands.Find(command => command.Client.ClientGuid == id).ToList();
        }

        public async Task CreateAsync(Command command)
        {
            await _commands.InsertOneAsync(command);
        }

        public void RemoveForClientGuid(string id)
        {
            _commands.DeleteMany(command => command.Client.ClientGuid == id);
        }

        public void RemoveAll()
        {
            _commands.DeleteMany(command => true);
        }

    }
}