using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Api.Infrastructure.Data.MongoDB.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<MongoGroup> _groups;
        private readonly IMapper _mapper;

        public GroupRepository(MongoConnectionOptions mongoConnectionOptions, IMapper mapper)
        {
            MongoClient client = new MongoClient(mongoConnectionOptions.ChatChainAPI.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoConnectionOptions.ChatChainAPI.DatabaseName);
            _groups = database.GetCollection<MongoGroup>("Groups");
            _mapper = mapper;
        }
    
        public async Task<GetGroupsGatewayResponse> GetForOwner(Guid ownerId)
        {
            try
            {
                IAsyncCursor<MongoGroup>
                    cursor = await _groups.FindAsync(group => group.OwnerId.Equals(ownerId));
                return new GetGroupsGatewayResponse(_mapper.Map<List<Group>>(await cursor.ToListAsync()), true);
            }
            catch (MongoException e)
            {
                return new GetGroupsGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<GetGroupsGatewayResponse> GetForClient(Guid clientId)
        {
            try
            {
                IAsyncCursor<MongoGroup>
                    cursor = await _groups.FindAsync(group => group.ClientIds.Contains(clientId));
                return new GetGroupsGatewayResponse(_mapper.Map<List<Group>>(await cursor.ToListAsync()), true);
            }
            catch (MongoException e)
            {
                return new GetGroupsGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<GetGroupGatewayResponse> Get(Guid id)
        {
            try
            {
                IAsyncCursor<MongoGroup> cursor = await _groups.FindAsync(group => group.Id == id);
                MongoGroup mongoIS4Group = await cursor.FirstOrDefaultAsync();
                return mongoIS4Group == null
                    ? new GetGroupGatewayResponse(null, false, new[] {new Error("404", "Group Not Found")})
                    : new GetGroupGatewayResponse(_mapper.Map<Group>(mongoIS4Group), true);
            }
            catch (MongoException e)
            {
                return new GetGroupGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<CreateGroupGatewayResponse> Create(Group group)
        {
            try
            {
                MongoGroup mongoIS4Group = _mapper.Map<MongoGroup>(group);
                await _groups.InsertOneAsync(mongoIS4Group);
                return new CreateGroupGatewayResponse(_mapper.Map<Group>(mongoIS4Group), true);
            }
            catch (MongoException e)
            {
                return new CreateGroupGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<UpdateGroupGatewayResponse> Update(Guid groupId, Group group)
        {
            try
            {
                IAsyncCursor<MongoGroup> cursor = await _groups.FindAsync(org => org.Id == groupId);
                MongoGroup mongoIS4Group = await cursor.FirstOrDefaultAsync();
                if (mongoIS4Group == null)
                    return new UpdateGroupGatewayResponse(null, false, new[] {new Error("404", "Group Not Found")});

                mongoIS4Group = _mapper.Map<MongoGroup>(group);
                await _groups.ReplaceOneAsync(org => org.Id.Equals(groupId), mongoIS4Group);
                return new UpdateGroupGatewayResponse(_mapper.Map<Group>(mongoIS4Group), true);
            }
            catch (MongoException e)
            {
                return new UpdateGroupGatewayResponse(null, false,
                    e.ErrorLabels.Select(label => new Error(e.HResult.ToString(), e.Message)));
            }
        }

        public async Task<BaseGatewayResponse> DeleteForOwner(Guid ownerId)
        {
            try
            {
                await _groups.DeleteManyAsync(config => config.OwnerId.Equals(ownerId));
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
                await _groups.DeleteOneAsync(config => config.Id.Equals(id));
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
    public class MongoGroup
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public IList<Guid> ClientIds { get; set; } = new List<Guid>();
    }
}