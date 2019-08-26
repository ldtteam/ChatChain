namespace ChatChainCommon.Config
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; } = "ChatChain";
    }
}