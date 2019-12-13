using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub.Core.DTO.GatewayResponses.Repositories.Requests;
using Hub.Core.Entities;

namespace Hub.Core.Interfaces.Gateways.Repositories
{
    public interface IRequestsRepository
    {
        Task<GetStatsRequestGatewayResponse> GetStatsRequest(Guid statsId);

        Task<CreateStatsRequestsGatewayResponse> CreateStatsRequests(IList<StatsRequest> statsRequest);
    }
}