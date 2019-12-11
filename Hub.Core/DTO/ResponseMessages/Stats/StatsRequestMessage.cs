using System;
using System.Collections.Generic;
using Api.Core.DTO;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages.Stats
{
    public class StatsRequestMessage : MessageResponse
    {
        public Guid ClientId { get; }
        
        public Guid RequestId { get; }
        
        public string StatsSection { get; }

        public StatsRequestMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public StatsRequestMessage(Guid clientId, Guid requestId, string statsSection, bool success = false) : base(success)
        {
            ClientId = clientId;
            RequestId = requestId;
            StatsSection = statsSection;
        }
    }
}