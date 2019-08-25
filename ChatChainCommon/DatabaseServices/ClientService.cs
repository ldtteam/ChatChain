using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        public async Task<IEnumerable<Client>> GetAsync()
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => true);
            return await cursor.ToListAsync();
        }
        
        public async Task<IEnumerable<Client>> GetFromOwnerIdAsync(string ownerId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.OwnerId == ownerId);
            return await cursor.ToListAsync();
        }
        
        public async Task<Client> GetAsync(ObjectId clientId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.Id == clientId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<Client> GetFromGuidAsync(string guid)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.ClientGuid == guid);
            return await cursor.FirstOrDefaultAsync();
        }
        
        public async Task<Client> GetFromIs4IdAsync(string clientId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.ClientId == clientId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ObjectId clientId, Client clientIn)
        {
            await _clients.ReplaceOneAsync(client => client.Id == clientId, clientIn);
        }

        public async Task RemoveAsync(ObjectId clientId)
        {
            await _clients.DeleteOneAsync(client => client.Id == clientId);
        }

        public async Task<ClientConfig> GetClientConfigAsync(ObjectId clientId)
        {
            Client client = await GetAsync(clientId);
            return await _services.GetRequiredService<ClientConfigService>().GetAsync(client.ClientConfigId);
        }
        
        public async Task CreateAsync(Client client)
        {
            await _clients.InsertOneAsync(client);
        }
        
        public async Task<List<Group>> GetGroupsAsync(ObjectId clientId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.ClientIds.Contains(clientId));
            return await cursor.ToListAsync();
        }

        public async Task AddGroupAsync(ObjectId clientId, ObjectId groupId, bool addClientToGroup = true)
        {
            GroupService groupService = _services.GetRequiredService<GroupService>();
            
            Client client = await GetAsync(clientId);
            Group group = await groupService.GetAsync(groupId);
            
            if (client == null || group == null) return;
            
            client.GroupIds.Add(group.Id);
            await UpdateAsync(client.Id, client);

            if (!addClientToGroup) return;

            await groupService.AddClientAsync(group.Id, client.Id, false);
        }
        
        public async Task RemoveGroupAsync(ObjectId clientId, ObjectId groupId, bool removeClientFromGroup = true)
        {
            GroupService groupService = _services.GetRequiredService<GroupService>();
            
            Client client = await GetAsync(clientId);
            Group group = await groupService.GetAsync(groupId);
            
            if (client == null || group == null) return;
            
            client.GroupIds.Remove(group.Id);
            await UpdateAsync(client.Id, client);

            if (!removeClientFromGroup) return;

            await groupService.RemoveClientAsync(group.Id, client.Id, false);
        }
    }
}