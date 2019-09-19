using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Organisation
{
    public class DeleteOrganisationResponse : UseCaseResponseMessage
    {
        public DeleteOrganisationResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public DeleteOrganisationResponse(bool checkedPermissions = false, bool success = false,
            string message = null) : base(checkedPermissions, success,
            message)
        {
        }
    }
}