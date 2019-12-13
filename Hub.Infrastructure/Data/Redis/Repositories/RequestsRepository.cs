using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DTO;
using Hub.Core.DTO.GatewayResponses.Repositories.Requests;
using Hub.Core.Entities;
using Hub.Core.Interfaces.Gateways.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Hub.Infrastructure.Data.Redis.Repositories
{
    public class RequestsRepository : IRequestsRepository
    {
        private readonly IDatabaseAsync _redisDatabase;

        public RequestsRepository(IConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase(2);
        }
        
        public async Task<GetStatsRequestGatewayResponse> GetStatsRequest(Guid statsId)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(statsId.ToString()))
            {
                StatsRequest statsRequest = JsonConvert.DeserializeObject<StatsRequest>(redisValue);
                if (statsRequest.RequestId.Equals(statsId))
                    return new GetStatsRequestGatewayResponse(statsRequest, true);
            }
            return new GetStatsRequestGatewayResponse(null, false, new[] {new Error("404", "Stats Request Not Found"), });
        }

        public async Task<CreateStatsRequestsGatewayResponse> CreateStatsRequests(IList<StatsRequest> statsRequests)
        {
            foreach (StatsRequest request in statsRequests)
            {
                await _redisDatabase.SetAddAsync(request.RequestId.ToString(), JsonConvert.SerializeObject(request));
                await _redisDatabase.KeyExpireAsync(request.RequestId.ToString(), DateTime.Now.AddDays(1));
            }
            
            return new CreateStatsRequestsGatewayResponse(statsRequests, true);
        }
    }
}