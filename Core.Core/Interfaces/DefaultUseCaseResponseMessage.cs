using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;

namespace Api.Core.Interfaces
{
    public abstract class DefaultUseCaseResponseMessage : UseCaseResponseMessage
    {
        public Organisation Organisation { get; }

        public OrganisationUser User { get; }

        protected DefaultUseCaseResponseMessage(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        protected DefaultUseCaseResponseMessage(Organisation organisation, OrganisationUser user,
            bool checkedPermissions = false, bool success = false, string message = null) : base(checkedPermissions,
            success, message)
        {
            Organisation = organisation;
            User = user;
        }
    }
}