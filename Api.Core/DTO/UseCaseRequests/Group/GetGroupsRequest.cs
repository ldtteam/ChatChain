using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Group
{
    public class GetGroupsRequest : DefaultUseCaseRequest
    {
        public GetGroupsRequest(string userId, Guid organisationId) : base(userId, organisationId)
        {
        }

        public GetGroupsRequest(Guid organisationId) : base(organisationId)
        {
        }
    }
}