using System;
using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages.Events
{
    public class ClientEventMessage : MessageResponse
    {
        
        public Client SendingClient { get; }
        
        public Guid ClientId { get; }
        
        public Group Group { get; }
        
        public IList<Group> Groups { get; } = new List<Group>();
        
        public string Event { get; }
        
        public Dictionary<string, string> EventData { get; } = new Dictionary<string, string>();

        public ClientEventMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public ClientEventMessage(Client sendingClient, Guid clientId, Group group, string eventName, Dictionary<string, string> eventData, bool success = false) : base(success)
        {
            SendingClient = sendingClient;
            ClientId = clientId;
            Group = group;
            Groups = new List<Group>();
            Event = eventName;
            EventData = eventData;
        }
        
        public ClientEventMessage(Client sendingClient, Guid clientId, IList<Group> groups, string eventName, Dictionary<string, string> eventData, bool success = false) : base(success)
        {
            SendingClient = sendingClient;
            ClientId = clientId;
            Group = null;
            Groups = groups;
            Event = eventName;
            EventData = eventData;
        }
    }
}