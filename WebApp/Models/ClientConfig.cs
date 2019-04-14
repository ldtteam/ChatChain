using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models
{
    /**
     * ---- IMPORTANT ----
     *
     * ALL CHANGES TO THIS FILE MUST BE RECIPROCATED IN THE ChatChainServer PROJECT
     *
     * ---- IMPORTANT ----
     */
    public class ClientConfig
    {
        public ObjectId Id { get; set; }

        [BsonElement("ClientId")]
        public ObjectId ClientId { get; set; }

        [BsonElement("ClientEventGroups")]
        public List<ObjectId> clientEventGroups { get; set; }

        [BsonElement("UserEventGroups")]
        public List<ObjectId> userEventGroups { get; set; }

    }
}