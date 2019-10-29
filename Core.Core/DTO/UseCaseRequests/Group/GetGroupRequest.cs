using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Group
{
    public class GetGroupRequest : DefaultUseCaseRequest
    {
        public Guid GroupId { get; }

        public GetGroupRequest(string userId, Guid organisationId, Guid groupId) : base(userId, organisationId)
        {
            GroupId = groupId;
        }

        public GetGroupRequest(Guid organisationId, Guid groupId) : base(organisationId)
        {
            GroupId = groupId;
        }
    }
}