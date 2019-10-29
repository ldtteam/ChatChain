using System;
using System.Collections.Generic;
using Hub.Core.Interfaces;
using Hub.Core.Entities;

namespace Hub.Core.DTO.UseCaseRequests
{
    public class UserEventRequest : DefaultUseCaseRequest
    {
        public ClientUser ClientUser { get; }
        
        public string Event { get; }
        
        public Dictionary<string, string> EventData { get; }

        public UserEventRequest(ClientUser clientUser, string eventName, Dictionary<string, string> eventData, Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
            ClientUser = clientUser;
            Event = eventName;
            EventData = eventData;
        }
    }
}