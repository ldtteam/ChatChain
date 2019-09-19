using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Api.Infrastructure.Data.MongoDB.Repositories
{
    public class ClientConfigRepository : IClientConfigRepository
    {
        private readonly IMongoCollection<MongoClientConfig> _clientConfigs;
        private readonly IMapper _mapper;

        public ClientConfigRepository(MongoConnectionOptions mongoConnectionOptions, IMapper mapper)
        {
            MongoClient client = new MongoClient(mongoConnectionOptions.ChatChainAPI.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnectionOptions.ChatChainAPI.DatabaseName);
            _clientConfigs = database.GetCollection<MongoClientConfig>("ClientConfigs");
            _mapper = mapper;
        }

        public async Task<GetClientConfigGatewayResponse> Get(Guid id)
        {
            try
            { 
                IAsyncCursor<MongoClientConfig> cursor = await _clientConfigs.FindAsync(c => c.Id.Equals(id));
                MongoClientConfig mongoClientConfig = await cursor.FirstOrDefaultAsync();
                return mongoClientConfig == null
                    ? new GetClientConfigGatewayResponse(null, false, new[] {new Error("404", "Client Config Not Found")})
                    : new GetClientConfigGatewayResponse(_mapper.Map<ClientConfig>(mongoClientConfig), true);
            }
            catch (MongoException e)
            {
                return new GetClientConfigGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<CreateClientConfigGatewayResponse> Create(ClientConfig clientConfig)
        {
            try
            {
                MongoClientConfig mongoClientConfig = _mapper.Map<MongoClientConfig>(clientConfig);
                await _clientConfigs.InsertOneAsync(mongoClientConfig);
                return new CreateClientConfigGatewayResponse(_mapper.Map<ClientConfig>(mongoClientConfig), true);
            }
            catch (MongoException e)
            {
                return new CreateClientConfigGatewayResponse(null, false, e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<UpdateClientConfigGatewayResponse> Update(Guid id, ClientConfig clientConfig)
        {
            try
            {
                IAsyncCursor<MongoClientConfig> cursor = await _clientConfigs.FindAsync(c => c.Id.Equals(id));
                MongoClientConfig mongoClientConfig = await cursor.FirstOrDefaultAsync();
                if (mongoClientConfig == null)
                    return new UpdateClientConfigGatewayResponse(null, false,
                        new[] {new Error("404", "Client Config Not Found")});

                mongoClientConfig = _mapper.Map<MongoClientConfig>(clientConfig);
                await _clientConfigs.ReplaceOneAsync(config => config.Id == id, mongoClientConfig);
                return new UpdateClientConfigGatewayResponse(_mapper.Map<ClientConfig>(mongoClientConfig), true);
            }
            catch (MongoException e)
            {
                return new UpdateClientConfigGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId)
        {
            try
            {
                await _clientConfigs.DeleteManyAsync(config => config.OwnerId.Equals(ownerId));
                return new BaseGatewayResponse(true);
            }
            catch (MongoException e)
            {
                return new BaseGatewayResponse(false,
                                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<BaseGatewayResponse> Delete(Guid id)
        {
            try
            {
                await _clientConfigs.DeleteOneAsync(config => config.Id.Equals(id));
                return new BaseGatewayResponse(true);
            }
            catch (MongoException e)
            {
                return new BaseGatewayResponse(false,
                                                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }
    }

    [BsonIgnoreExtraElements]
    public class MongoClientConfig
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)] public Guid OwnerId { get; set; }

        [BsonRepresentation(BsonType.String)] public IEnumerable<Guid> ClientEventGroups { get; set; }

        [BsonRepresentation(BsonType.String)] public IEnumerable<Guid> UserEventGroups { get; set; }
    }
}