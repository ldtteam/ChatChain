using System;
using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Group
{
    public class CreateGroupRequest : DefaultUseCaseRequest
    {
        public string Name { get; }

        public string Description { get; }

        public IList<Guid> ClientIds { get; }

        public CreateGroupRequest(string userId, Guid organisationId, string name, string description,
            IList<Guid> clientIds) : base(userId, organisationId)
        {
            Name = name;
            Description = description;
            ClientIds = clientIds;
        }

        public CreateGroupRequest(Guid organisationId, string name, string description, IList<Guid> clientIds) : base(
            organisationId)
        {
            Name = name;
            Description = description;
            ClientIds = clientIds;
        }
    }
}