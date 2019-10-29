using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Invite
{
    public class UseInviteResponse : DefaultUseCaseResponseMessage
    {
        public UseInviteResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        public UseInviteResponse(Entities.Organisation organisation, Entities.OrganisationUser user,
            bool checkedPermissions = false, bool success = false, string message = null) : base(organisation, user,
            checkedPermissions, success, message)
        {
        }
    }
}