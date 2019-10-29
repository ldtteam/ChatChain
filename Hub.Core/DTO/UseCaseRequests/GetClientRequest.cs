using System;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseRequests
{
    public class GetClientRequest : DefaultUseCaseRequest
    {
        public GetClientRequest(Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
        }
    }
}