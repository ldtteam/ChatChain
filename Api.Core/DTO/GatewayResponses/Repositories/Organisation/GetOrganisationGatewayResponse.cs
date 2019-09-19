using System.Collections.Generic;
using Api.Core.Entities;

namespace Api.Core.DTO.GatewayResponses.Repositories.Organisation
{
    public class GetOrganisationGatewayResponse : BaseGatewayResponse
    {
        public OrganisationDetails Organisation { get; }

        public GetOrganisationGatewayResponse(OrganisationDetails organisation, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Organisation = organisation;
        }
    }
}