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

        public async Task<IEnumerable<Group>> GetAsync()
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => true);
            return await cursor.ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetFromOwnerAsync(string ownerId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.OwnerId == ownerId);
            return await cursor.ToListAsync();
        }
        
        public async Task<Group> GetAsync(ObjectId groupId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.Id == groupId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<Group> GetFromGuidAsync(string guid)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.GroupId == guid);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ObjectId groupId, Group groupIn)
        {
            await _groups.ReplaceOneAsync(group => group.Id == groupId, groupIn);
        }

        public async Task RemoveAsync(ObjectId groupId)
        {
            await _groups.DeleteOneAsync(group => group.Id == groupId);
        }
        
        public async Task CreateAsync(Group group)
        {
            await _groups.InsertOneAsync(group);
        }

        public async Task<IEnumerable<Client>> GetClientsAsync(ObjectId groupId)
        {
            IAsyncCursor<Client> cursor = await _clients.FindAsync(client => client.GroupIds.Contains(groupId));
            return await cursor.ToListAsync();
        }

        public async Task AddClientAsync(ObjectId groupId, ObjectId clientId, bool addGroupToClient = true)
        {
            ClientService clientService = _services.GetRequiredService<ClientService>();
            
            Group group = await GetAsync(groupId);
            Client client = await clientService.GetAsync(clientId);
            
            if (group == null || client == null) return;

            group.ClientIds.Add(client.Id);
            await UpdateAsync(group.Id, group);
            
            if (!addGroupToClient) return;

            await clientService.AddGroupAsync(client.Id, group.Id, false);
        }
        
        public async Task RemoveClientAsync(ObjectId groupId, ObjectId clientId, bool removeGroupFromClient = true)
        {
            ClientService clientService = _services.GetRequiredService<ClientService>();
            
            Group group = await GetAsync(groupId);
            Client client = await clientService.GetAsync(clientId);
            
            if (group == null || client == null) return;
            
            group.ClientIds.Remove(client.Id);
            await UpdateAsync(group.Id, group);
            
            if (!removeGroupFromClient) return;

            await clientService.RemoveGroupAsync(client.Id, group.Id, false);
        }
    }
}