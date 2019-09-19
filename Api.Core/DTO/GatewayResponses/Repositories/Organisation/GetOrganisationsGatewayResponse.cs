using System.Collections.Generic;
using Api.Core.Entities;

namespace Api.Core.DTO.GatewayResponses.Repositories.Organisation
{
    public class GetOrganisationsGatewayResponse : BaseGatewayResponse
    {
        public IEnumerable<OrganisationDetails> Organisations { get; }

        public GetOrganisationsGatewayResponse(IEnumerable<OrganisationDetails> organisations, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Organisations = organisations;
        }
    }
}