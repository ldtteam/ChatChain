using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Collections;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Driver;

namespace IdentityServer.Services
{
      public class IdentityUserCollection<TUser> : IIdentityUserCollection<TUser> where TUser : MongoUser
	{
	    private readonly IMongoCollection<TUser> _users;

        public IdentityUserCollection(string connectionString, string databaseName, string collectionName)
        {
	        MongoClient client = new MongoClient(connectionString);

	        IMongoDatabase database = client.GetDatabase(databaseName);
	        _users = database.GetCollection<TUser>(collectionName);
        }

		public async Task<TUser> FindByEmailAsync(string normalizedEmail)
		{
			IAsyncCursor<TUser> find = await _users.FindAsync(u => u.NormalizedEmail == normalizedEmail);
			return await find.FirstOrDefaultAsync();
		}

		public async Task<TUser> FindByUserNameAsync(string username)
		{
			IAsyncCursor<TUser> find = await _users.FindAsync(u => u.UserName == username);
			return await find.FirstOrDefaultAsync();
		}

		public async Task<TUser> FindByNormalizedUserNameAsync(string normalizedUserName)
		{
			IAsyncCursor<TUser> find = await _users.FindAsync(u => u.NormalizedUserName == normalizedUserName);
			return await find.FirstOrDefaultAsync();
		}

		public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
		{
			IAsyncCursor<TUser> find = await _users.FindAsync(u =>
				u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));

			return await find.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<TUser>> FindUsersByClaimAsync(string claimType, string claimValue)
		{
			IAsyncCursor<TUser> find = await _users.FindAsync(u => u.Claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue));

			return await find.ToListAsync();
		}

		public async Task<IEnumerable<TUser>> FindUsersInRoleAsync(string roleName)
		{
		    FilterDefinition<TUser> filter = Builders<TUser>.Filter.AnyEq(x => x.Roles, roleName);
            IAsyncCursor<TUser> res = await _users.FindAsync(filter);
		    return res.ToEnumerable();
        }

        public async Task<IEnumerable<TUser>> GetAllAsync()
        {
            IAsyncCursor<TUser> res = await _users.FindAsync(x=>true);
            return await res.ToListAsync();
        }

        public async Task<TUser> CreateAsync(TUser obj)
        {
            await _users.InsertOneAsync(obj);
            return obj;
        }

	    public async Task UpdateAsync(TUser obj)
	    {
		    await _users.ReplaceOneAsync(x => x.Id == obj.Id, obj);
	    }

	    public async Task DeleteAsync(TUser obj)
        {
	        await _users.DeleteOneAsync(x => x.Id == obj.Id);
        }

        public async Task<TUser> FindByIdAsync(string itemId)
        {
	        IAsyncCursor<TUser> find = await _users.FindAsync(x => x.Id == itemId);
		    return await find.FirstOrDefaultAsync();
	    }
	}
}