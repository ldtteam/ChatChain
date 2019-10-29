using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Client
{
    public class CreateClientRequest : DefaultUseCaseRequest
    {
        public string Name { get; }

        public string Description { get; }

        public CreateClientRequest(string userId, Guid organisationId, string name, string description) : base(userId,
            organisationId)
        {
            Name = name;
            Description = description;
        }

        public CreateClientRequest(Guid organisationId, string name, string description) : base(organisationId)
        {
            Name = name;
            Description = description;
        }
    }
}