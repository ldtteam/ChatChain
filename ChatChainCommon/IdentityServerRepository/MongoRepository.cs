using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ChatChainCommon.Config;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatChainCommon.IdentityServerRepository
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

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);
        }
        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            IMongoCollection<T> collection = _database.GetCollection<T>(typeof(T).Name);
            BsonDocument filter = new BsonDocument();
            long totalCount = collection.CountDocuments(filter);
            return totalCount > 0;
        }
        
        public void Update<T>(Expression<Func<T, bool>> expression, T item) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).ReplaceOne(expression, item);
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