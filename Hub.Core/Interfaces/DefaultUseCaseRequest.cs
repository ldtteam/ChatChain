using System;
using Api.Core.Interfaces;

namespace Hub.Core.Interfaces
{
    public abstract class DefaultUseCaseRequest : IUseCaseRequest
    {
        public Guid ClientId { get; set; }
        
        public bool SendToSelf { get; set; }

        protected DefaultUseCaseRequest(Guid clientId, bool sendToSelf = false)
        {
            ClientId = clientId;
            SendToSelf = sendToSelf;
        }
    }
}