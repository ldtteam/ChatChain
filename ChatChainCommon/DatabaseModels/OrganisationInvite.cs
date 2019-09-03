using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainCommon.DatabaseModels
{
    public class OrganisationInvite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string OrganisationId { get; set; }
        
        public string Token { get; set; }
        
        public string Email { get; set; }
    }
}