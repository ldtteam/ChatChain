using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Client
{
    public class UpdateClientResponse : DefaultUseCaseResponseMessage
    {
        public Entities.Client Client { get; }

        public UpdateClientResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public UpdateClientResponse(Entities.Client client, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            Client = client;
        }
    }
}