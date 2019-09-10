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
    public class Client
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid OwnerId { get; set; }

        public string ClientName { get; set; }
        
        public string ClientDescription { get; set; }
    }
}