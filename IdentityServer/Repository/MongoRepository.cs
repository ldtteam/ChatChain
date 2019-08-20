using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.Config;
using IdentityServer.Interface;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Repository
{
    public class MongoRepository : IRepository
    {
        private static IMongoDatabase _database;

        /// <summary>
        /// This Contructor leverages .NET Core built-in DI
        /// </summary>
        public MongoRepository(MongoConnections mongoConnections)
        {
            IMongoClient client = new MongoClient(mongoConnections.IdentityServerConnection.ConnectionString);
            _database = client.GetDatabase(mongoConnections.IdentityServerConnection.DatabaseName);
        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            return _database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public void Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);
        }
        public T Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection.CountDocuments(filter);
            return totalCount > 0;
        }

        public void Add<T>(T item) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).InsertOne(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).InsertMany(items);
        }
    }
}