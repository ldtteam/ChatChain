using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatChainServer.Models.MessageObjects
{
    public class ClientEventMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ClientEventType Event { get; set; }
        public Group Group { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
        public Dictionary<string, string> ExtraEventData { get; set; }
    }
}