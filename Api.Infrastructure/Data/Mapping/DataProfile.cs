using System.Collections.Generic;
using Api.Core.Entities;
using Api.Infrastructure.Data.MongoDB.Repositories;
using AutoMapper;

namespace Api.Infrastructure.Data.Mapping
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<ClientConfig, MongoClientConfig>().ConstructUsing(config => new MongoClientConfig
            {
                Id = config.Id,
                OwnerId = config.OwnerId,
                ClientEventGroups = config.ClientEventGroups,
                UserEventGroups = config.UserEventGroups
            });
            CreateMap<MongoClientConfig, ClientConfig>().ConstructUsing(mConfig => new ClientConfig
            {
                Id = mConfig.Id,
                OwnerId = mConfig.OwnerId,
                ClientEventGroups = mConfig.ClientEventGroups,
                UserEventGroups = mConfig.UserEventGroups
            });

            CreateMap<OrganisationDetails, MongoOrganisation>().ConstructUsing((organisation, context) => new MongoOrganisation
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Owner = organisation.Name,
                Users = context.Mapper.Map<List<MongoOrganisationUser>>(organisation.Users)
            });
            CreateMap<MongoOrganisation, OrganisationDetails>().ConstructUsing((mOrganisation, context) => new OrganisationDetails
            {
                Id = mOrganisation.Id,
                Name = mOrganisation.Name,
                Owner = mOrganisation.Owner,
                Users = context.Mapper.Map<List<OrganisationUser>>(mOrganisation.Users)
            });

            CreateMap<OrganisationUser, MongoOrganisationUser>().ConstructUsing(user => new MongoOrganisationUser
            {
                Id = user.Id,
                Permissions = user.Permissions
            });
            CreateMap<MongoOrganisationUser, OrganisationUser>().ConstructUsing(mUser => new OrganisationUser
            {
                Id = mUser.Id,
                Permissions = mUser.Permissions
            });

            CreateMap<Client, MongoChatChainClient>().ConstructUsing(client => new MongoChatChainClient
            {
                Id = client.Id,
                OwnerId = client.OwnerId,
                Description = client.Description,
                Name = client.Name
            });
            CreateMap<MongoChatChainClient, Client>().ConstructUsing(mClient => new Client
            {
                Id = mClient.Id,
                OwnerId = mClient.OwnerId,
                Description = mClient.Description,
                Name = mClient.Name
            });
            
            CreateMap<IS4Client, MongoIS4Client>().ConstructUsing(client => new MongoIS4Client
            {
                Id = client.Id,
                OwnerId = client.OwnerId,
                AllowedGrantTypes = client.AllowedGrantTypes,
                AllowedScopes = client.AllowedScopes,
                AllowOfflineAccess = client.AllowOfflineAccess
            });
            CreateMap<MongoIS4Client, IS4Client>().ConstructUsing(mClient => new IS4Client
            {
                Id = mClient.Id,
                OwnerId = mClient.OwnerId,
                AllowedGrantTypes = mClient.AllowedGrantTypes,
                AllowedScopes = mClient.AllowedScopes,
                AllowOfflineAccess = mClient.AllowOfflineAccess
            });
            
            CreateMap<MongoIS4Client, IdentityServer4.Models.Client>().ConstructUsing(mClient => new IdentityServer4.Models.Client
            {
                ClientId = mClient.Id.ToString(),
                AllowedGrantTypes = mClient.AllowedGrantTypes,
                ClientSecrets = mClient.Secrets,
                AllowedScopes = mClient.AllowedScopes,
                AllowOfflineAccess = mClient.AllowOfflineAccess
            });

            CreateMap<Group, MongoGroup>().ConstructUsing(group => new MongoGroup
            {
                Id = group.Id,
                OwnerId = group.OwnerId,
                Description = group.Description,
                Name = group.Name,
                ClientIds = group.ClientIds
            });
            CreateMap<MongoGroup, Group>().ConstructUsing(mGroup => new Group
            {
                Id = mGroup.Id,
                OwnerId = mGroup.OwnerId,
                Description = mGroup.Description,
                Name = mGroup.Name,
                ClientIds = mGroup.ClientIds
            });
        }
    }
}