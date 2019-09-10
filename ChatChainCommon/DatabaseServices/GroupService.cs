using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using MongoDB.Driver;

namespace ChatChainCommon.DatabaseServices
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GroupService : IGroupService
    {
        private readonly IMongoCollection<Group> _groups;

        public GroupService(MongoConnections mongoConnections)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _groups = database.GetCollection<Group>("Groups");
        }

        public async Task<IEnumerable<Group>> GetAsync()
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => true);
            return await cursor.ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetFromOwnerIdAsync(Guid orgId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.OwnerId == orgId);
            return await cursor.ToListAsync();
        }
        
        public async Task<IEnumerable<Group>> GetFromClientIdAsync(Guid clientId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.ClientIds.Contains(clientId));
            return await cursor.ToListAsync();
        }
        
        public async Task<Group> GetAsync(Guid groupId)
        {
            IAsyncCursor<Group> cursor = await _groups.FindAsync(group => group.Id == groupId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid groupId, Group groupIn)
        {
            await _groups.ReplaceOneAsync(group => group.Id == groupId, groupIn);
        }

        public async Task RemoveAsync(Guid groupId)
        {
            await _groups.DeleteOneAsync(group => group.Id == groupId);
        }
        
        public async Task CreateAsync(Group group)
        {
            await _groups.InsertOneAsync(group);
        }
    }
}