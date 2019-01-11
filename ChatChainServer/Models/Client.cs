using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models
{
    public class Client
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("ClientId")]
        public int ClientId { get; set; }
        
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }
        
        [BsonElement("ClientGuid")]
        public string ClientGuid { get; set; }
        
        [BsonElement("GroupIds")]
        public List<ObjectId> GroupIds { get; } = new List<ObjectId>();
    }
}