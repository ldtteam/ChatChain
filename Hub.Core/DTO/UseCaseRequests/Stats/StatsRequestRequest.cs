using System;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseRequests.Stats
{
    public class StatsRequestRequest : DefaultUseCaseRequest
    {
        public Guid RequestedClient { get; set; }
        
        public Guid RequestedGroup { get; set; }
        
        public string StatsSection { get; set; }

        public StatsRequestRequest(Guid requestedClient, Guid requestedGroup, string statsSection, Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
            RequestedClient = requestedClient;
            RequestedGroup = requestedGroup;
            StatsSection = statsSection;
        }
    }
}