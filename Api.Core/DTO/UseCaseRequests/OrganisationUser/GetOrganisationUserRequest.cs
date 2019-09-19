using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.OrganisationUser
{
    public class GetOrganisationUserRequest : DefaultUseCaseRequest
    {
        public string OrganisationUserId { get; }

        public GetOrganisationUserRequest(string userId, Guid organisationId, string organisationUserId) : base(userId,
            organisationId)
        {
            OrganisationUserId = organisationUserId;
        }

        public GetOrganisationUserRequest(Guid organisationId, string organisationUserId) : base(organisationId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}