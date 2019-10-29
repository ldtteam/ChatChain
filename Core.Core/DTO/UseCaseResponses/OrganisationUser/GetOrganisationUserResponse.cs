using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.OrganisationUser
{
    public class GetOrganisationUserResponse : DefaultUseCaseResponseMessage
    {
        public Entities.OrganisationUser RequestedUser { get; }

        public GetOrganisationUserResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public GetOrganisationUserResponse(Entities.OrganisationUser requestedUser, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            RequestedUser = requestedUser;
        }
    }
}