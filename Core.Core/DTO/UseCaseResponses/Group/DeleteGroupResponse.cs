using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Group
{
    public class DeleteGroupResponse : DefaultUseCaseResponseMessage
    {
        public DeleteGroupResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public DeleteGroupResponse(Entities.Organisation organisation, Entities.OrganisationUser user,
            bool checkedPermissions = false, bool success = false, string message = null) : base(organisation, user,
            checkedPermissions, success, message)
        {
        }
    }
}