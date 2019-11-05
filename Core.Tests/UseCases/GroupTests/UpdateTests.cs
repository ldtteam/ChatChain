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
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateGroup_True()
        {
            // Arrange \\

            // Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Group Arrange

            Guid groupId = Guid.NewGuid();

            // Update Values
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";
            IList<Guid> clientIds = new List<Guid> {Guid.NewGuid()};

            Group mockGroup = new Group
                {Id = groupId, OwnerId = orgId, Name = groupName, Description = groupDescription};

            bool updateRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(groupId)), It.IsAny<Group>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateGroupGatewayResponse(mockGroup, true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            UpdateGroupUseCase useCase =
                new UpdateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<UpdateGroupResponse> mockOutputPort = new MockOutputPort<UpdateGroupResponse>();


            // Act \\
            bool response = await useCase.HandleAsync(
                new UpdateGroupRequest(orgId, groupId, groupName, groupDescription, clientIds), mockOutputPort);


            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.Message);

            Assert.True(updateRan);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Group.Id, groupId);

            // Assert Updated Values.
            Assert.Equal(mockOutputPort.Response.Group.Name, groupName);
            Assert.Equal(mockOutputPort.Response.Group.Description, groupDescription);
            Assert.Equal(mockOutputPort.Response.Group.ClientIds, clientIds);
        }

        [Fact]
        public async Task UpdateGroup_OrganisationNonExistent_False()
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

            Guid groupId = Guid.NewGuid();

            Group mockGroup = new Group {Id = groupId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(groupId)), It.IsAny<Group>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateGroupGatewayResponse(mockGroup, true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            UpdateGroupUseCase useCase =
                new UpdateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<UpdateGroupResponse> mockOutputPort = new MockOutputPort<UpdateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateGroupRequest(orgId, groupId, "", "", new List<Guid>()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateGroup_NonExistent_False()
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

            Guid groupId = Guid.NewGuid();

            Group mockGroup = new Group {Id = groupId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(groupId)), It.IsAny<Group>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateGroupGatewayResponse(mockGroup, true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(null, false, new[] {new Error("404", "Group Not Found")}));

            // Use Case and OutputPort

            UpdateGroupUseCase useCase =
                new UpdateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<UpdateGroupResponse> mockOutputPort = new MockOutputPort<UpdateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateGroupRequest(orgId, groupId, "", "", new List<Guid>()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateGroup_NotInOrganisation_False()
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

            Guid groupId = Guid.NewGuid();

            Group mockGroup = new Group {Id = groupId, OwnerId = Guid.NewGuid()};

            bool updateRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(groupId)), It.IsAny<Group>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateGroupGatewayResponse(mockGroup, true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            UpdateGroupUseCase useCase =
                new UpdateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<UpdateGroupResponse> mockOutputPort = new MockOutputPort<UpdateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateGroupRequest(orgId, groupId, "", "", new List<Guid>()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateGroup_RepoUpdateErrors_False()
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

            Guid groupId = Guid.NewGuid();

            Group mockGroup = new Group {Id = groupId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(groupId)), It.IsAny<Group>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateGroupGatewayResponse(null, false,
                    new[] {new Error("500", "Group Update Failed")}));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            UpdateGroupUseCase useCase =
                new UpdateGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<UpdateGroupResponse> mockOutputPort = new MockOutputPort<UpdateGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateGroupRequest(orgId, groupId, "", "", new List<Guid>()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(updateRan);
        }
    }
}