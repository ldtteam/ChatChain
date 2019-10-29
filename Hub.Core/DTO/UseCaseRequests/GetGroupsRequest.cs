using System;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseRequests
{
    public class GetGroupsRequest : DefaultUseCaseRequest
    {
        public GetGroupsRequest(Guid clientId, bool sendToSelf = false) : base(clientId, sendToSelf)
        {
        }
    }
}