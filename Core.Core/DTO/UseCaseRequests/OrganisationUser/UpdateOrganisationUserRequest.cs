using System;
using System.Collections.Generic;
using Api.Core.Entities;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.OrganisationUser
{
    public class UpdateOrganisationUserRequest : DefaultUseCaseRequest
    {
        public string OrganisationUserId { get; }

        public IList<OrganisationPermissions> Permissions { get; }

        public UpdateOrganisationUserRequest(string userId, Guid organisationId, string organisationUserId,
            IList<OrganisationPermissions> permissions) : base(userId, organisationId)
        {
            OrganisationUserId = organisationUserId;
            Permissions = permissions;
        }

        public UpdateOrganisationUserRequest(Guid organisationId, string organisationUserId,
            IList<OrganisationPermissions> permissions) : base(organisationId)
        {
            OrganisationUserId = organisationUserId;
            Permissions = permissions;
        }
    }
}