using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChatChainCommon.DatabaseServices
{
    public class OrganisationService
    {
        private readonly IMongoCollection<Organisation> _organisations;
        private readonly IDatabaseAsync _redisDatabase;

        public OrganisationService(MongoConnections mongoConnections, IConnectionMultiplexer redisConnection)
        {
            MongoClient client = new MongoClient(mongoConnections.ChatChainGroups.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnections.ChatChainGroups.DatabaseName);
            _organisations = database.GetCollection<Organisation>("Organisations");
            _redisDatabase = redisConnection.GetDatabase();
        }

        public async Task<IEnumerable<Organisation>> GetAsync()
        {
            IAsyncCursor<Organisation> cursor = await _organisations.FindAsync(org => true);
            return await cursor.ToListAsync();
        }

        public async Task<Organisation> GetAsync(ObjectId orgId)
        {
            IAsyncCursor<Organisation> cursor = await _organisations.FindAsync(org => org.Id == orgId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Organisation>> GetForUserAsync(string userId)
        {
            /*IList<Organisation> organisations = 
                (await GetAsync())
                .Where(organisation => 
                    organisation.Users.Select(orgUser => orgUser.IdentityUserId)
                        .Contains(userId)).ToList();*/
            IAsyncCursor<Organisation> cursor = await _organisations.FindAsync(org => org.Users.ContainsKey(userId));

            return await cursor.ToListAsync();
        }

        public async Task UpdateAsync(ObjectId orgId, Organisation orgIn)
        {
            await _organisations.ReplaceOneAsync(org => org.Id == orgId, orgIn);
        }

        public async Task RemoveAsync(ObjectId orgId)
        {
            await _organisations.DeleteOneAsync(org => org.Id == orgId);
        }

        public async Task CreateAsync(Organisation org)
        {
            await _organisations.InsertOneAsync(org);
        }

        public async Task<OrganisationInvite> GetInviteAsync(ObjectId orgId, string inviteToken)
        {
            return (from redisValue in await _redisDatabase
                .SetMembersAsync(orgId.ToString()) select JsonConvert.DeserializeObject<OrganisationInvite>(redisValue))
                .FirstOrDefault(orgInvite => orgInvite.Token == inviteToken);
        }

        public async Task RemoveInviteAsync(ObjectId orgId, string inviteToken)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(orgId.ToString()))
            {
                OrganisationInvite orgInvite = JsonConvert.DeserializeObject<OrganisationInvite>(redisValue);
                if (orgInvite.Token != inviteToken) continue;
                
                await _redisDatabase.SetRemoveAsync(orgId.ToString(), redisValue);
                return;
            }
        }

        public async Task CreateInviteAsync(OrganisationInvite invite)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(invite.OrganisationId))
            {
                OrganisationInvite orgInvite = JsonConvert.DeserializeObject<OrganisationInvite>(redisValue);
                if (invite.Email != orgInvite.Email) continue;
                
                await _redisDatabase.SetRemoveAsync(invite.OrganisationId, redisValue);
                break;
            }
            await _redisDatabase.SetAddAsync(invite.OrganisationId, JsonConvert.SerializeObject(invite));
            await _redisDatabase.KeyExpireAsync(invite.OrganisationId, DateTime.Now.AddDays(1));
        }

    }
}