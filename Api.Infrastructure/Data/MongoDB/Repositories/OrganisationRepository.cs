using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Api.Infrastructure.Data.MongoDB.Repositories
{
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly IMongoCollection<MongoOrganisation> _organisations;
        private readonly IMapper _mapper;

        public OrganisationRepository(MongoConnectionOptions mongoConnectionOptions, IMapper mapper)
        {
            MongoClient client = new MongoClient(mongoConnectionOptions.ChatChainAPI.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnectionOptions.ChatChainAPI.DatabaseName);
            _organisations = database.GetCollection<MongoOrganisation>("Organisations");
            _mapper = mapper;
        }

        public async Task<GetOrganisationsGatewayResponse> GetForUser(string userId)
        {
            try
            {
                if (userId == null)
                {
                    IAsyncCursor<MongoOrganisation> cursor = await _organisations.FindAsync(org => true);
                    return new GetOrganisationsGatewayResponse(_mapper.Map<List<OrganisationDetails>>(await cursor.ToListAsync()), true);
                }
                else
                {
                    FilterDefinition<MongoOrganisation> filter = Builders<MongoOrganisation>.Filter.ElemMatch(org => org.Users, user => user.Id.Equals(userId));
                    IAsyncCursor<MongoOrganisation>
                        cursor = await _organisations.FindAsync(filter);
                    return new GetOrganisationsGatewayResponse(_mapper.Map<List<OrganisationDetails>>(await cursor.ToListAsync()), true);
                }
            }
            catch (MongoException e)
            {
                return new GetOrganisationsGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<GetOrganisationGatewayResponse> Get(Guid id)
        {
            try
            {
                IAsyncCursor<MongoOrganisation> cursor = await _organisations.FindAsync(org => org.Id == id);
                MongoOrganisation mongoOrganisation = await cursor.FirstOrDefaultAsync();
                return mongoOrganisation == null
                    ? new GetOrganisationGatewayResponse(null, false, new[] {new Error("404", "Organisation Not Found")})
                    : new GetOrganisationGatewayResponse(_mapper.Map<OrganisationDetails>(mongoOrganisation), true);
            }
            catch (MongoException e)
            {
                return new GetOrganisationGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<CreateOrganisationGatewayResponse> Create(OrganisationDetails organisation)
        {
            try
            {
                MongoOrganisation mongoOrganisation = _mapper.Map<MongoOrganisation>(organisation);
                await _organisations.InsertOneAsync(mongoOrganisation);
                return new CreateOrganisationGatewayResponse(_mapper.Map<OrganisationDetails>(mongoOrganisation), true);
            }
            catch (MongoException e)
            {
                return new CreateOrganisationGatewayResponse(null, false,
                                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<UpdateOrganisationGatewayResponse> Update(Guid organisationId, OrganisationDetails organisation)
        {
            try
            {
                IAsyncCursor<MongoOrganisation> cursor = await _organisations.FindAsync(org => org.Id == organisationId);
                MongoOrganisation mongoOrganisation = await cursor.FirstOrDefaultAsync();
                if (mongoOrganisation == null)
                    return new UpdateOrganisationGatewayResponse(null, false, new[] {new Error("404", "Organisation Not Found")});

                mongoOrganisation = _mapper.Map<MongoOrganisation>(organisation);
                await _organisations.ReplaceOneAsync(org => org.Id.Equals(organisationId), mongoOrganisation);
                return new UpdateOrganisationGatewayResponse(_mapper.Map<OrganisationDetails>(mongoOrganisation), true);
            }
            catch (MongoException e)
            {
                return new UpdateOrganisationGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<BaseGatewayResponse> Delete(Guid id)
        {
            try
            {
                await _organisations.DeleteOneAsync(config => config.Id.Equals(id));
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
    public class MongoOrganisation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public IEnumerable<MongoOrganisationUser> Users { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class MongoOrganisationUser
    {
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public IList<OrganisationPermissions> Permissions { get; set; } = new List<OrganisationPermissions>();
    }
}