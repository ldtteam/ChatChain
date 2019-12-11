using System;

namespace Hub.Core.Entities
{
    public class StatsRequest
    {
        public Guid RequestId { get; set; }

        public Guid SendingClient { get; set; }

        public Guid RequestedClient { get; set; }
    }
}