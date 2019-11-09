using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Client
{
    public class GetClientsResponse : DefaultUseCaseResponseMessage
    {
        public IEnumerable<Entities.Client> Clients { get; }

        public GetClientsResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        public GetClientsResponse(IEnumerable<Entities.Client> clients, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            Clients = clients;
        }
    }
}