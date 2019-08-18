using Newtonsoft.Json;

namespace ChatChainServer.Models.MessageObjects
{
    public class CommandResponseMessage
    {
        public string Id { get; set; }
        public string Response { get; set; }
        public Group SendingGroup { get; set; }
        public Client SendingClient { get; set; }
    }
}