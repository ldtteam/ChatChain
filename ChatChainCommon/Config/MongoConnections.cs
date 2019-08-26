namespace ChatChainCommon.Config
{
    public class MongoConnections
    {
        public MongoOptions IdentityConnection { get; set; }
        public MongoOptions IdentityServerConnection { get; set; }
        public MongoOptions ChatChainGroups { get; set; }
    }
}