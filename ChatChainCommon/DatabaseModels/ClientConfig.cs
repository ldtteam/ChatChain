using System.Collections.Generic;
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
    public class ClientConfig
    {
        public ObjectId Id { get; set; }

        [BsonElement("ClientId")]
        public ObjectId ClientId { get; set; }

        [BsonElement("ClientEventGroups")]
        public List<ObjectId> ClientEventGroups { get; set; } = new List<ObjectId>();

        [BsonElement("UserEventGroups")]
        public List<ObjectId> UserEventGroups { get; set; } = new List<ObjectId>();

    }
}