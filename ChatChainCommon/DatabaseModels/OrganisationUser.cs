using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatChainCommon.DatabaseModels
{
    public class OrganisationUser
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public IList<OrganisationPermissions> Permissions { get; set; } = new List<OrganisationPermissions>();
    }
}