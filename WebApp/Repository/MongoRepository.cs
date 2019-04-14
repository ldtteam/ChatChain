using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApp.Repository
{
    public class MongoRepository : IRepository
    {
        private static IMongoDatabase _database;

        /// <summary>
        /// This Contructor leverages  .NET Core built-in DI
        /// </summary>
        public MongoRepository()
        {
            IMongoClient client = new MongoClient(Environment.GetEnvironmentVariable("IDENTITY_SERVER_DATABASE_CONNECTION"));
            _database = client.GetDatabase(Environment.GetEnvironmentVariable("IDENTITY_SERVER_DATABASE"));
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
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection.CountDocuments(filter);
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