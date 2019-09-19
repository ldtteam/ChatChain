using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainCommon.DatabaseModels
{
    public class Organisation
    {
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Owner { get; set; }

        public IDictionary<string, OrganisationUser> Users { get; set; }
    }
}