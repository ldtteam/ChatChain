using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Group
{
    public class DeleteGroupRequest : DefaultUseCaseRequest
    {
        public Guid GroupId { get; }

        public DeleteGroupRequest(string userId, Guid organisationId, Guid groupId) : base(userId, organisationId)
        {
            GroupId = groupId;
        }

        public DeleteGroupRequest(Guid organisationId, Guid groupId) : base(organisationId)
        {
            GroupId = groupId;
        }
    }
}