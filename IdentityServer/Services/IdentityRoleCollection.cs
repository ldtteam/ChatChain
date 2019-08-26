using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Collections;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Driver;

namespace IdentityServer.Services
{
    public class IdentityRoleCollection<TRole> : IIdentityRoleCollection<TRole> where TRole : MongoRole
    {
        private readonly IMongoCollection<TRole> _roles;

        public IdentityRoleCollection(string connectionString, string databaseName, string collectionName)
        {
            MongoClient client = new MongoClient(connectionString);

            IMongoDatabase database = client.GetDatabase(databaseName);
            _roles = database.GetCollection<TRole>(collectionName);
        }

        public async Task<TRole> FindByNameAsync(string normalizedName)
        {
            IAsyncCursor<TRole> find = await _roles.FindAsync(x => x.NormalizedName == normalizedName);
            return await find.FirstOrDefaultAsync();
        }

        public async Task<TRole> FindByIdAsync(string roleId)
        {
            IAsyncCursor<TRole> find = await _roles.FindAsync(x => x.Id == roleId);
            return await find.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TRole>> GetAllAsync()
        {
            return (await _roles.FindAsync(x => true)).ToEnumerable();
        }

        public async Task<TRole> CreateAsync(TRole obj)
        {
            await _roles.InsertOneAsync(obj);
            return obj;
        }

        public Task UpdateAsync(TRole obj) => _roles.ReplaceOneAsync(x=>x.Id == obj.Id, obj);

        public Task DeleteAsync(TRole obj) => _roles.DeleteOneAsync(x => x.Id == obj.Id);
    }
}