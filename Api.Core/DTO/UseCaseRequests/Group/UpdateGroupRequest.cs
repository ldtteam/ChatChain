using System;
using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Group
{
    public class UpdateGroupRequest : DefaultUseCaseRequest
    {
        public Guid GroupId { get; }

        public string Name { get; }

        public string Description { get; }

        public IList<Guid> ClientIds { get; }

        public UpdateGroupRequest(string userId, Guid organisationId, Guid groupId, string name, string description,
            IList<Guid> clientIds) : base(userId, organisationId)
        {
            GroupId = groupId;
            Name = name;
            Description = description;
            ClientIds = clientIds;
        }

        public UpdateGroupRequest(Guid organisationId, Guid groupId, string name, string description,
            IList<Guid> clientIds) : base(organisationId)
        {
            GroupId = groupId;
            Name = name;
            Description = description;
            ClientIds = clientIds;
        }
    }
}