using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Classification;

namespace IdentityServer_WebApp.Repository
{
    /// <summary>
    /// Basic interface with a few methods for adding, deleting, and querying data.
    /// </summary>
    public interface IRepository
    {
        System.Linq.IQueryable<T> All<T>() where T : class, new();
        IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new();
        T Single<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void Add<T>(T item) where T : class, new();
        void Add<T>(IEnumerable<T> items) where T : class, new();
        bool CollectionExists<T>() where T : class, new();
        void Update<T>(Expression<Func<T, bool>> expression, T item) where T : class, new();
    }
}