using System;
using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;
using Hub.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages
{
    public class GenericMessageMessage : MessageResponse
    {
        public Client SendingClient { get; }
        
        public Guid ClientId { get; }
        
        public Group Group { get; }
        
        public ClientUser ClientUser { get; }
        
        public string Message { get; }

        public GenericMessageMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public GenericMessageMessage(Client sendingClient, Guid clientId, Group group, ClientUser clientUser, string message, bool success = false) : base(success)
        {
            SendingClient = sendingClient;
            ClientId = clientId;
            Group = group;
            ClientUser = clientUser;
            Message = message;
        }
    }
}