using System;

namespace Api.Core.Entities
{
    public class Client
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}