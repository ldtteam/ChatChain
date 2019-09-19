using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Client
{
    public class GetClientsRequest : DefaultUseCaseRequest
    {
        public GetClientsRequest(string userId, Guid organisationId) : base(userId, organisationId)
        {
        }

        public GetClientsRequest(Guid organisationId) : base(organisationId)
        {
        }
    }
}