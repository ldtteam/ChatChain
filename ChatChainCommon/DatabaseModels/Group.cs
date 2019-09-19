using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatChainCommon.DatabaseModels
{
    /**
     * ---- IMPORTANT ----
     *
     * ALL CHANGES TO THIS FILE MUST BE RECIPROCATED IN THE ChatChainServer PROJECT
     *
     * ---- IMPORTANT ----
     */
    public class Group
    {
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string GroupName { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }
        
        public string GroupDescription { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public List<Guid> ClientIds { get; set; } = new List<Guid>();
    }
}