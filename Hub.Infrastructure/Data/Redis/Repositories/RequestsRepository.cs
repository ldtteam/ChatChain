using System;
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

        public async Task<CreateStatsRequestGatewayResponse> CreateStatsRequest(StatsRequest statsRequest)
        {
            await _redisDatabase.SetAddAsync(statsRequest.RequestId.ToString(), JsonConvert.SerializeObject(statsRequest));
            await _redisDatabase.KeyExpireAsync(statsRequest.RequestId.ToString(), DateTime.Now.AddDays(1));
            
            return new CreateStatsRequestGatewayResponse(statsRequest, true);
        }
    }
}