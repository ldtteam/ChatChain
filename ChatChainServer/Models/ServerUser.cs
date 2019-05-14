using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models
{
    public class ServerUser
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }

        public List<ObjectId> ClientUsers { get; set; } = new List<ObjectId>();
    }
}