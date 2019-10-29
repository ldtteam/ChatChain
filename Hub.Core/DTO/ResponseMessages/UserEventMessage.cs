using System;
using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;
using Hub.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages
{
    public class UserEventMessage : MessageResponse
    {
        
        public Client SendingClient { get; }
        
        public ClientUser ClientUser { get; }
        
        public Guid ClientId { get; }
        
        public Group Group { get; }
        
        public IList<Group> Groups { get; }
        
        public string Event { get; }
        
        public Dictionary<string, string> EventData { get; }

        public UserEventMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public UserEventMessage(Client sendingClient, ClientUser clientUser, Guid clientId, Group group, string eventName, Dictionary<string, string> eventData, bool success = false) : base(success)
        {
            SendingClient = sendingClient;
            ClientUser = clientUser;
            ClientId = clientId;
            Group = group;
            Groups = new List<Group>();
            Event = eventName;
            EventData = eventData;
        }
        
        public UserEventMessage(Client sendingClient, ClientUser clientUser, Guid clientId, IList<Group> groups, string eventName, Dictionary<string, string> eventData, bool success = false) : base(success)
        {
            SendingClient = sendingClient;
            ClientUser = clientUser;
            ClientId = clientId;
            Group = null;
            Groups = groups;
            Event = eventName;
            EventData = eventData;
        }
    }
}