using System.Collections.Generic;
using ChatChainServer.Models.CommandObjects.Arguments;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models.CommandObjects
{
    public class Command
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("Name")]
        public string Name { get; set; }
        
        [BsonElement("Client")]
        public Client Client { get; set; }
        
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }

        [BsonElement("CommandArguments")]
        public List<CommandArgument> CommandArguments { get; set; }
    }
}