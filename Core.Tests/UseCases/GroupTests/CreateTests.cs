using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
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

namespace Api.Tests.UseCases.GroupTests
{
    public class CreateTests
    {
        [Fact]
        public async Task CreateGroup_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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
                Description = groupDescription,
                ClientIds = clientIds
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

            bool response =
                await useCase.HandleAsync(new CreateGroupRequest(orgId, groupName, groupDescription, clientIds),
                    mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Group.OwnerId, orgId);

            // Assert Group Values
            Assert.Equal(mockOutputPort.Response.Group.Name, groupName);
            Assert.Equal(mockOutputPort.Response.Group.Description, groupDescription);
            Assert.Equal(mockOutputPort.Response.Group.ClientIds, clientIds);
        }

        [Fact]
        public async Task CreateGroup_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

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

            bool response =
                await useCase.HandleAsync(new CreateGroupRequest(orgId, groupName, groupDescription, clientIds),
                    mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task CreateGroup_RepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Group Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            List<Guid> clientIds = new List<Guid>();

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Create(It.IsAny<Group>()))
                .ReturnsAsync(new CreateGroupGatewayResponse(null, false,
                    new[] {new Error("500", "Group Create Failed")}));

            // Use Case and OutputPort

            CreateGroupUseCase useCase =
                new CreateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<CreateGroupResponse> mockOutputPort = new MockOutputPort<CreateGroupResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateGroupRequest(orgId, groupName, groupDescription, clientIds),
                    mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}