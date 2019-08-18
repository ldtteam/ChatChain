using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatChainServer.Models.MessageObjects
{
    public class UserEventMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UserEventType Event { get; set; }
        public Group Group { get; set; }
        public ClientUser User { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
        public Dictionary<string, string> ExtraEventData { get; set; }
    }
}