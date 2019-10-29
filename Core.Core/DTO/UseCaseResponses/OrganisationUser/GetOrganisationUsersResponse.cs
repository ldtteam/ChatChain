using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.OrganisationUser
{
    public class GetOrganisationUsersResponse : DefaultUseCaseResponseMessage
    {
        public IEnumerable<Entities.OrganisationUser> RequestedUsers { get; }

        public GetOrganisationUsersResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        public GetOrganisationUsersResponse(IEnumerable<Entities.OrganisationUser> requestedUsers,
            Entities.Organisation organisation, Entities.OrganisationUser user, bool checkedPermissions = false,
            bool success = false, string message = null) : base(organisation, user, checkedPermissions, success,
            message)
        {
            RequestedUsers = requestedUsers;
        }
    }
}