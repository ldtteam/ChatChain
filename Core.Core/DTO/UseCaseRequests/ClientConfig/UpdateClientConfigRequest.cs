using System;
using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.ClientConfig
{
    public class UpdateClientConfigRequest : DefaultUseCaseRequest
    {
        public Guid ClientConfigId { get; }

        public IEnumerable<Guid> ClientEventGroups { get; }

        public IEnumerable<Guid> UserEventGroups { get; }

        public UpdateClientConfigRequest(string userId, Guid organisationId, Guid clientConfigId,
            IEnumerable<Guid> clientEventGroups, IEnumerable<Guid> userEventGroups) : base(userId, organisationId)
        {
            ClientConfigId = clientConfigId;
            ClientEventGroups = clientEventGroups;
            UserEventGroups = userEventGroups;
        }

        public UpdateClientConfigRequest(Guid organisationId, Guid clientConfigId, IEnumerable<Guid> clientEventGroups,
            IEnumerable<Guid> userEventGroups) : base(organisationId)
        {
            ClientConfigId = clientConfigId;
            ClientEventGroups = clientEventGroups;
            UserEventGroups = userEventGroups;
        }
    }
}