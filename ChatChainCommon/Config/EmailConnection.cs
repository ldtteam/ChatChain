namespace ChatChainCommon.Config
{
    public class EmailConnection
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public bool Ssl { get; set; } = true;
    }
}