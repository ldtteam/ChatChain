using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Invite
{
    public class CreateInviteResponse : DefaultUseCaseResponseMessage
    {
        public Entities.Invite Invite { get; }

        public CreateInviteResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        public CreateInviteResponse(Entities.Invite invite, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            Invite = invite;
        }
    }
}