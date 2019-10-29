using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.OrganisationUser
{
    public class GetOrganisationUsersRequest : DefaultUseCaseRequest
    {
        public GetOrganisationUsersRequest(string userId, Guid organisationId) : base(userId, organisationId)
        {
        }

        public GetOrganisationUsersRequest(Guid organisationId) : base(organisationId)
        {
        }
    }
}