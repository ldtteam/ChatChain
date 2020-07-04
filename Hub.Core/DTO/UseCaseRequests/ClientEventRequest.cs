using System;
using System.Collections.Generic;
using Hub.Core.Interfaces;
using Hub.Core.Entities;

namespace Hub.Core.DTO.UseCaseRequests
{
    public class ClientEventRequest : DefaultUseCaseRequest
    {
        public string Event { get; set; }
        
        public Dictionary<string, string> EventData { get; set; }

        public ClientEventRequest(string eventName, Dictionary<string, string> eventData, Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
            Event = eventName;
            EventData = eventData;
        }
    }
}