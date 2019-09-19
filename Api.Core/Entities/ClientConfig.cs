using System;
using System.Collections.Generic;

namespace Api.Core.Entities
{
    public class ClientConfig
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public IEnumerable<Guid> ClientEventGroups { get; set; } = new List<Guid>();

        public IEnumerable<Guid> UserEventGroups { get; set; } = new List<Guid>();
    }
}