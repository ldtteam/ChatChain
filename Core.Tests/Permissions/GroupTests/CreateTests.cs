using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Group;
using Api.Core.DTO.UseCaseResponses.Group;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.Group;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.GroupTests
{
    public class CreateTests
    {
        [Fact]
        public async Task CreateGroup_NonOrgUser_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Id = orgId
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Group Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            List<Guid> clientIds = new List<Guid>();

            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription
            };

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Create(It.IsAny<Group>()))
                .ReturnsAsync(new CreateGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            CreateGroupUseCase useCase =
                new CreateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<CreateGroupResponse> mockOutputPort = new MockOutputPort<CreateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateGroupRequest(Guid.NewGuid().ToString(), orgId, groupName, groupDescription, clientIds),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task CreateGroup_WrongPermission_False(OrganisationPermissions permission)
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = userId.ToString(),
                        Permissions = new List<OrganisationPermissions>
                        {
                            permission
                        }
                    }
                }
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Group Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            List<Guid> clientIds = new List<Guid>();

            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription
            };

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Create(It.IsAny<Group>()))
                .ReturnsAsync(new CreateGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            CreateGroupUseCase useCase =
                new CreateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<CreateGroupResponse> mockOutputPort = new MockOutputPort<CreateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateGroupRequest(userId.ToString(), orgId, groupName, groupDescription, clientIds),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.All)]
        public async Task CreateGroup_CorrectPermission_True(OrganisationPermissions permission)
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = userId.ToString(),
                        Permissions = new List<OrganisationPermissions>
                        {
                            permission
                        }
                    }
                }
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Group Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            List<Guid> clientIds = new List<Guid>();

            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription
            };

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Create(It.IsAny<Group>()))
                .ReturnsAsync(new CreateGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            CreateGroupUseCase useCase =
                new CreateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<CreateGroupResponse> mockOutputPort = new MockOutputPort<CreateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateGroupRequest(userId.ToString(), orgId, groupName, groupDescription, clientIds),
                mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async Task CreateGroup_OrgOwner_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = userId.ToString(),
                Users = new[] {new OrganisationUser {Id = userId.ToString()}}
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Group Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            List<Guid> clientIds = new List<Guid>();

            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription
            };

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Create(It.IsAny<Group>()))
                .ReturnsAsync(new CreateGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            CreateGroupUseCase useCase =
                new CreateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<CreateGroupResponse> mockOutputPort = new MockOutputPort<CreateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateGroupRequest(userId.ToString(), orgId, groupName, groupDescription, clientIds),
                mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}