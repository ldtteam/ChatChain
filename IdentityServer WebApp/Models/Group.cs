using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer_WebApp.Models
{
    public class Group
    {
        public ObjectId Id { get; set; }

        [BsonElement("GroupId")]
        public string GroupId { get; set; }
        
        [BsonElement("GroupName")]
        public string GroupName { get; set; }
        
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; }
        
        [BsonElement("ClientIds")]
        public List<ObjectId> ClientIds { get; set; } = new List<ObjectId>();
    }
}