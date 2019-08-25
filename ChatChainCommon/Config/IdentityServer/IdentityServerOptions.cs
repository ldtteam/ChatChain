namespace ChatChainCommon.Config.IdentityServer
{
    public class IdentityServerOptions
    {
        public string ServerUrl { get; set; }
        public string ServerOrigin { get; set; }
        public string SigningPath { get; set; }
        public string SigningPassword { get; set; }
    }
}