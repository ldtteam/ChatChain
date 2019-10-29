using System;
using System.Collections.Generic;

namespace Api.Core.Entities
{
    public class IS4Client
    {
        public Guid Id { get; set; }
        
        public Guid OwnerId { get; set; }

        public ICollection<string> AllowedGrantTypes { get; set; } = new List<string>();

        public ICollection<string> AllowedScopes { get; set; } = new List<string>();
        
        public bool AllowOfflineAccess { get; set; }
    }
}