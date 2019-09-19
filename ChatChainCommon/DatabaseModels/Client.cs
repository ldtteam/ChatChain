using System;
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
    public class Client
    {
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        [Required]
        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }

        public string ClientName { get; set; }
        
        public string ClientDescription { get; set; }
    }
}