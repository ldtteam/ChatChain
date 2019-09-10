using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatChainCommon.DatabaseModels
{
    public class Organisation
    {
        [Required]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Owner { get; set; }

        public IDictionary<string, OrganisationUser> Users { get; set; }
    }
}