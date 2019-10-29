using System;
using System.Collections.Generic;
using Api.Core.Entities;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Organisation
{
    public class GetOrganisationsResponse : UseCaseResponseMessage
    {
        public IEnumerable<OrganisationDetails> Organisations { get; }

        public IDictionary<Guid, Entities.OrganisationUser> Users { get; }

        public GetOrganisationsResponse(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public GetOrganisationsResponse(IEnumerable<OrganisationDetails> organisations,
            IDictionary<Guid, Entities.OrganisationUser> users, bool checkedPermissions = false, bool success = false,
            string message = null) : base(checkedPermissions, success, message)
        {
            Organisations = organisations;
            Users = users;
        }
    }
}