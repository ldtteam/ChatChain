using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.Interfaces.Gateways.Repositories;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Client = Api.Core.Entities.Client;

namespace Api.Infrastructure.Data.MongoDB.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMongoCollection<MongoChatChainClient> _clients;
        private readonly IMapper _mapper;

        public ClientRepository(MongoConnectionOptions mongoConnectionOptions, IMapper mapper)
        {
            MongoClient client = new MongoClient(mongoConnectionOptions.ChatChainAPI.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnectionOptions.ChatChainAPI.DatabaseName);
            _clients = database.GetCollection<MongoChatChainClient>("Clients");
            _mapper = mapper;
        }
    
        public async Task<GetClientsGatewayResponse> GetForOwner(Guid ownerId)
        {
            try
            {
                IAsyncCursor<MongoChatChainClient>
                    cursor = await _clients.FindAsync(org => org.OwnerId.Equals(ownerId));
                return new GetClientsGatewayResponse(_mapper.Map<List<Client>>(await cursor.ToListAsync()), true);
            }
            catch (MongoException e)
            {
                return new GetClientsGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<GetClientGatewayResponse> Get(Guid id)
        {
            try
            {
                IAsyncCursor<MongoChatChainClient> cursor = await _clients.FindAsync(client => client.Id == id);
                MongoChatChainClient mongoClient = await cursor.FirstOrDefaultAsync();
                return mongoClient == null
                    ? new GetClientGatewayResponse(null, false, new[] {new Error("404", "Client Not Found")})
                    : new GetClientGatewayResponse(_mapper.Map<Client>(mongoClient), true);
            }
            catch (MongoException e)
            {
                return new GetClientGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<CreateClientGatewayResponse> Create(Client client)
        {
            try
            {
                MongoChatChainClient mongoClient = _mapper.Map<MongoChatChainClient>(client);
                await _clients.InsertOneAsync(mongoClient);
                return new CreateClientGatewayResponse(_mapper.Map<Client>(mongoClient), true);
            }
            catch (MongoException e)
            {
                return new CreateClientGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<UpdateClientGatewayResponse> Update(Guid clientId, Client client)
        {
            try
            {
                IAsyncCursor<MongoChatChainClient> cursor = await _clients.FindAsync(org => org.Id == clientId);
                MongoChatChainClient mongoClient = await cursor.FirstOrDefaultAsync();
                if (mongoClient == null)
                    return new UpdateClientGatewayResponse(null, false, new[] {new Error("404", "Client Not Found")});

                mongoClient = _mapper.Map<MongoChatChainClient>(client);
                await _clients.ReplaceOneAsync(org => org.Id.Equals(clientId), mongoClient);
                return new UpdateClientGatewayResponse(_mapper.Map<Client>(mongoClient), true);
            }
            catch (MongoException e)
            {
                return new UpdateClientGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId)
        {
            try
            {
                await _clients.DeleteManyAsync(config => config.OwnerId.Equals(ownerId));
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
                await _clients.DeleteOneAsync(config => config.Id.Equals(id));
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
    public class MongoChatChainClient
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}