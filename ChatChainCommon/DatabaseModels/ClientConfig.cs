using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        public List<Guid> ClientEventGroups { get; set; } = new List<Guid>();

        public List<Guid> UserEventGroups { get; set; } = new List<Guid>();

    }
}