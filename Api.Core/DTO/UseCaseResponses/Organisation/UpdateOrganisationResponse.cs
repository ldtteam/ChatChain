using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Organisation
{
    public class UpdateOrganisationResponse : UseCaseResponseMessage
    {
        public Entities.OrganisationDetails Organisation { get; }

        public Entities.OrganisationUser User { get; }

        public UpdateOrganisationResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public UpdateOrganisationResponse(Entities.OrganisationDetails organisation, Entities.OrganisationUser user,
            bool checkedPermissions = false, bool success = false,
            string message = null) : base(checkedPermissions, success,
            message)
        {
            Organisation = organisation;
            User = user;
        }
    }
}