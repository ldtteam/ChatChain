using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatChainCommon.DatabaseModels
{
    public class Organisation
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("Name")]
        public string Name { get; set; }
        
        [BsonElement("Owner")]
        public string Owner { get; set; }

        [BsonElement("Users")]
        [JsonConverter(typeof(StringEnumConverter))]
        public IDictionary<string, OrganisationUser> Users { get; set; }
    }
}