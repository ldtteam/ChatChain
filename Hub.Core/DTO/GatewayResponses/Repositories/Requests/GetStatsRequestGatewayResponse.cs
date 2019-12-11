using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Hub.Core.Entities;

namespace Hub.Core.DTO.GatewayResponses.Repositories.Requests
{
    public class GetStatsRequestGatewayResponse : BaseGatewayResponse
    {
        public StatsRequest StatsRequest { get; }

        public GetStatsRequestGatewayResponse(StatsRequest statsRequest, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            StatsRequest = statsRequest;
        }
    }
}