// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Api.Infrastructure.Data.MongoDB
{
    public class MongoConnectionOptions
    {
        public class Connection
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public Connection ChatChainAPI { get; set; }
        
        public Connection IdentityServer { get; set; }
    }
}