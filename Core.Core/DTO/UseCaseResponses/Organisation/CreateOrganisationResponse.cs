using System.Collections.Generic;
using Api.Core.Entities;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Organisation
{
    public class CreateOrganisationResponse : UseCaseResponseMessage
    {
        public OrganisationDetails Organisation { get; }

        public Entities.OrganisationUser User { get; }

        public CreateOrganisationResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public CreateOrganisationResponse(OrganisationDetails organisation, Entities.OrganisationUser user,
            bool checkedPermissions = false, bool success = false,
            string message = null) : base(checkedPermissions, success,
            message)
        {
            Organisation = organisation;
            User = user;
        }
    }
}