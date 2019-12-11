using System;
using System.Collections.Generic;
using Api.Core.DTO;
using Hub.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages.Stats
{
    public class StatsResponseMessage : MessageResponse
    {
        public Guid ClientId { get; }
        
        public Guid RequestId { get; }
        
        public StatsObject StatsObject { get; }

        public StatsResponseMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public StatsResponseMessage(Guid clientId, Guid requestId, StatsObject statsObject, bool success = false) : base(success)
        {
            ClientId = clientId;
            RequestId = requestId;
            StatsObject = statsObject;
        }
    }
}