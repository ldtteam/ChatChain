using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.IS4Client;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Client = IdentityServer4.Models.Client;

namespace Api.Infrastructure.Data.MongoDB.Repositories
{
    public class IS4ClientRepository : IIS4ClientRepository, IClientStore
    {
        private readonly IMongoCollection<MongoIS4Client> _clients;
        private readonly IMapper _mapper;

        public IS4ClientRepository(MongoConnectionOptions mongoConnectionOptions, IMapper mapper)
        {
            MongoClient client = new MongoClient(mongoConnectionOptions.IdentityServer.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnectionOptions.IdentityServer.DatabaseName);
            _clients = database.GetCollection<MongoIS4Client>("Client");
            _mapper = mapper;
        }
        
        public async Task<CreateIS4ClientGatewayResponse> Create(IS4Client is4Client, string password)
        {
            try
            {
                MongoIS4Client mongoClient = _mapper.Map<MongoIS4Client>(is4Client);
                mongoClient.ClientSecrets = new List<Secret>{new Secret(password.Sha256())};
                await _clients.InsertOneAsync(mongoClient);
                return new CreateIS4ClientGatewayResponse(true);
            }
            catch (MongoException e)
            {
                return new CreateIS4ClientGatewayResponse(false, e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
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

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            IAsyncCursor<MongoIS4Client> cursor = await _clients.FindAsync(c => c.Id.ToString().Equals(clientId));
            MongoIS4Client mongoIS4Client = await cursor.FirstOrDefaultAsync();

            return _mapper.Map<Client>(mongoIS4Client);
        }
    }
    
    [BsonIgnoreExtraElements]
    public class MongoIS4Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public Guid ClientId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }

        public ICollection<string> AllowedGrantTypes { get; set; } = new List<string>();

        public ICollection<Secret> ClientSecrets { get; set; } = new List<Secret>();

        public ICollection<string> AllowedScopes { get; set; } = new List<string>();
        
        public bool AllowOfflineAccess { get; set; }
    }
}