using System;
using System.Threading.Tasks;
using ChatChainServer.Models.MessageObjects;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainServer.Services
{
    public class CommandExecutionService
    {
        private readonly IMongoCollection<CommandExecutionMessage> _commandExecutions;
        
        public CommandExecutionService(IConfiguration config)
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
            _commandExecutions = database.GetCollection<CommandExecutionMessage>("CommandExecutions");
        }
        
        public CommandExecutionMessage Get(string id)
        {
            return _commandExecutions.Find(command => command.Id == id).FirstOrDefault();
        }

        public async Task CreateAsync(CommandExecutionMessage message)
        {
            await _commandExecutions.InsertOneAsync(message);
        }

        public async Task RemoveAsync(CommandExecutionMessage message)
        {
            await _commandExecutions.DeleteOneAsync(m => m.Id == message.Id);
        }
    }
}