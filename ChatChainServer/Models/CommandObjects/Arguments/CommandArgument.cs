using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatChainServer.Models.CommandObjects.Arguments
{
    public class CommandArgument
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ArgumentTypeEnum Type { get; set; }

        [BsonIgnore]
        public object Value { get; set; }
    }
}