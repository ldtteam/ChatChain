using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainServer.Models
{
    public class ClientUser
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("Name")]
        public string Name { get; set; }
        
        [BsonElement("UniqueId")]
        public string UniqueId { get; set; }
        public string NickName { get; set; }
        public string Colour { get; set; }
        
        public List<ClientRank> ClientRanks { get; set; }
        
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; } 
    }
}