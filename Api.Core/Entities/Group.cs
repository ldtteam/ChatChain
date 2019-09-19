using System;
using System.Collections.Generic;

namespace Api.Core.Entities
{
    public class Group
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public IList<Guid> ClientIds { get; set; } = new List<Guid>();
    }
}