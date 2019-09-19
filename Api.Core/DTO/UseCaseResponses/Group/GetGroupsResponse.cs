using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Group
{
    public class GetGroupsResponse : DefaultUseCaseResponseMessage
    {
        public IEnumerable<Entities.Group> Groups { get; }

        public GetGroupsResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public GetGroupsResponse(IEnumerable<Entities.Group> groups, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            Groups = groups;
        }
    }
}