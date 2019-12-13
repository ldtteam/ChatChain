using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Hub.Core.Entities;

namespace Hub.Core.DTO.GatewayResponses.Repositories.Requests
{
    public class CreateStatsRequestsGatewayResponse : BaseGatewayResponse
    {
        public IList<StatsRequest> StatsRequests { get; }

        public CreateStatsRequestsGatewayResponse(IList<StatsRequest> statsRequests, bool success = false, IEnumerable<Error> errors = null) : base(success, errors)
        {
            StatsRequests = statsRequests;
        }
    }
}