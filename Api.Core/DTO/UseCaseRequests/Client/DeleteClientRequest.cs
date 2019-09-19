using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Client
{
    public class DeleteClientRequest : DefaultUseCaseRequest
    {
        public Guid ClientId { get; }

        public DeleteClientRequest(string userId, Guid organisationId, Guid clientId) : base(userId, organisationId)
        {
            ClientId = clientId;
        }

        public DeleteClientRequest(Guid organisationId, Guid clientId) : base(organisationId)
        {
            ClientId = clientId;
        }
    }
}