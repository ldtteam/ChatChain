using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Client
{
    public class GetClientRequest : DefaultUseCaseRequest
    {
        public Guid ClientId { get; }

        public GetClientRequest(string userId, Guid organisationId, Guid clientId) : base(userId, organisationId)
        {
            ClientId = clientId;
        }

        public GetClientRequest(Guid organisationId, Guid clientId) : base(organisationId)
        {
            ClientId = clientId;
        }
    }
}