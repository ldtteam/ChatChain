using System;
using Hub.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseRequests.Stats
{
    public class StatsResponseRequest : DefaultUseCaseRequest
    {
        public Guid RequestId { get; set; }
        
        public StatsObject StatsObject { get; set; }

        public StatsResponseRequest(Guid requestId, StatsObject statsObject, Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
            RequestId = requestId;
            StatsObject = statsObject;
        }
    }
}