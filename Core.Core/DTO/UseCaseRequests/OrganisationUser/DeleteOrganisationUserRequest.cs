using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.OrganisationUser
{
    public class DeleteOrganisationUserRequest : DefaultUseCaseRequest
    {
        public string OrganisationUserId { get; }

        public DeleteOrganisationUserRequest(string userId, Guid organisationId, string organisationUserId) : base(
            userId, organisationId)
        {
            OrganisationUserId = organisationUserId;
        }

        public DeleteOrganisationUserRequest(Guid organisationId, string organisationUserId) : base(organisationId)
        {
            OrganisationUserId = organisationUserId;
        }
    }
}