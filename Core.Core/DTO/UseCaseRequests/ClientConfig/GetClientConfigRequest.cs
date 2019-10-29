using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.ClientConfig
{
    public class GetClientConfigRequest : DefaultUseCaseRequest
    {
        public Guid ClientConfigId { get; }

        public GetClientConfigRequest(string userId, Guid organisationId, Guid clientConfigId) : base(userId,
            organisationId)
        {
            ClientConfigId = clientConfigId;
        }

        public GetClientConfigRequest(Guid organisationId, Guid clientConfigId) : base(organisationId)
        {
            ClientConfigId = clientConfigId;
        }
    }
}