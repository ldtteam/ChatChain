using System;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Api.Infrastructure.Data.Redis.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly IDatabaseAsync _redisDatabase;

        public InviteRepository(IConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase(1);
        }
    
        public async Task<GetInviteGatewayResponse> Get(Guid organisationId, string token)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(organisationId.ToString()))
            {
                Invite invite = JsonConvert.DeserializeObject<Invite>(redisValue);
                if (invite.Token.Equals(token))
                    return new GetInviteGatewayResponse(invite, true);
            }
            return new GetInviteGatewayResponse(null, false, new[] {new Error("404", "Invite Not Found")});
        }

        public async Task<CreateInviteGatewayResponse> Create(Invite invite)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(invite.OrganisationId.ToString()))
            {
                Invite orgInvite = JsonConvert.DeserializeObject<Invite>(redisValue);
                if (invite.Email != orgInvite.Email) continue;
                
                await _redisDatabase.SetRemoveAsync(invite.OrganisationId.ToString(), redisValue);
                break;
            }
            await _redisDatabase.SetAddAsync(invite.OrganisationId.ToString(), JsonConvert.SerializeObject(invite));
            await _redisDatabase.KeyExpireAsync(invite.OrganisationId.ToString(), DateTime.Now.AddDays(1));
            
            return new CreateInviteGatewayResponse(invite, true);
        }

        public async Task<BaseGatewayResponse> Delete(Guid organisationId, string token)
        {
            foreach (RedisValue redisValue in await _redisDatabase.SetMembersAsync(organisationId.ToString()))
            {
                Invite orgInvite = JsonConvert.DeserializeObject<Invite>(redisValue);
                if (orgInvite.Token != token) continue;
                
                await _redisDatabase.SetRemoveAsync(organisationId.ToString(), redisValue);
                return new BaseGatewayResponse(true);
            }
            return new BaseGatewayResponse(false, new[] {new Error("404", "Invite Not Found")});
        }
    }
}