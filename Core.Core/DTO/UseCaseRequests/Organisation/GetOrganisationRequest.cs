using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Organisation
{
    public class GetOrganisationRequest : DefaultUseCaseRequest
    {
        public GetOrganisationRequest(string userId, Guid organisationId) : base(userId, organisationId)
        {
        }

        public GetOrganisationRequest(Guid organisationId) : base(organisationId)
        {
        }
    }
}