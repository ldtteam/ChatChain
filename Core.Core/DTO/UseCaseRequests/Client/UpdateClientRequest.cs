using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Client
{
    public class UpdateClientRequest : DefaultUseCaseRequest
    {
        public Guid ClientId { get; }

        public string Name { get; }

        public string Description { get; }

        public UpdateClientRequest(string userId, Guid organisationId, Guid clientId, string name, string description) :
            base(userId, organisationId)
        {
            ClientId = clientId;
            Name = name;
            Description = description;
        }

        public UpdateClientRequest(Guid organisationId, Guid clientId, string name, string description) : base(
            organisationId)
        {
            ClientId = clientId;
            Name = name;
            Description = description;
        }
    }
}