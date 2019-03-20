using System.Collections.Generic;

namespace ChatChainServer.Models.MessageObjects
{
    public class ClientEventMessage
    {
        public string Event { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
        public Dictionary<string, string> ExtraEventData { get; set; }
    }
}