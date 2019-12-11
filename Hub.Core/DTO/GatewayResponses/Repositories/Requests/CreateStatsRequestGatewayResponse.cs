using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Hub.Core.Entities;

namespace Hub.Core.DTO.GatewayResponses.Repositories.Requests
{
    public class CreateStatsRequestGatewayResponse : BaseGatewayResponse
    {
        public StatsRequest StatsRequest { get; }

        public CreateStatsRequestGatewayResponse(StatsRequest statsRequest, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            StatsRequest = statsRequest;
        }
    }
}