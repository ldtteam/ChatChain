namespace IdentityServer.Config
{
    public class MongoConnections
    {
        public MongoOptions IdentityConnection { get; set; }
        public MongoOptions IdentityServerConnection { get; set; }
    }
}