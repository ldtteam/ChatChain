using ChatChainServer.Models.CommandObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models.MessageObjects
{
    public class CommandExecutionMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonIgnore]
        public Command Command { get; set; }
        
        [BsonIgnore]
        public ClientUser SendingClientUser { get; set; }
        
        [BsonElement("SendingGroup")]
        public Group SendingGroup { get; set; }
        
        [BsonElement("SendingClient")]
        public Client SendingClient { get; set; }
    }
}