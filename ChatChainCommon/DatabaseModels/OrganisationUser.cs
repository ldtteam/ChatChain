using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainCommon.DatabaseModels
{
    public class OrganisationUser
    {
        [BsonRepresentation(BsonType.String)]
        public IList<OrganisationPermissions> Permissions { get; set; } = new List<OrganisationPermissions>();
    }
}