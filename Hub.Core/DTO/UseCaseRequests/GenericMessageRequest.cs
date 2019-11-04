using System;
using Hub.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseRequests
{
    public class GenericMessageRequest : DefaultUseCaseRequest
    {
        public Guid GroupId { get; set; }
        
        public ClientUser ClientUser { get; set; }
        
        public string Message { get; set; }

        public GenericMessageRequest(Guid groupId, ClientUser clientUser, string message, Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
            GroupId = groupId;
            ClientUser = clientUser;
            Message = message;
        }
    }
}