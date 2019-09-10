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
    public class Group
    {
        [Required]
        public Guid Id { get; set; }

        public string GroupName { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
        
        public string GroupDescription { get; set; }
        
        public List<Guid> ClientIds { get; set; } = new List<Guid>();
    }
}