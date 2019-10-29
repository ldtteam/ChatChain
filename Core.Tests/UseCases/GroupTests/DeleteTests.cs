using System;
using System.Linq;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
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
    public class DeleteTests
    {
        [Fact]
        public async void DeleteGroup_True()
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

            //For the first "control" test we'll set all the variables and assert them down below.
            const string groupName = "Test Group";
            const string groupDescription = "Test Description";

            Group mockGroup = new Group
            {
                Id = groupId,
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription
            };

            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            DeleteGroupUseCase useCase =
                new DeleteGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteGroupResponse> mockOutputPort = new MockOutputPort<DeleteGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);

            Assert.True(deleteRan);
        }

        [Fact]
        public async void DeleteGroup_OrganisationNonExistent_False()
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

            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            DeleteGroupUseCase useCase =
                new DeleteGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteGroupResponse> mockOutputPort = new MockOutputPort<DeleteGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
        }

        [Fact]
        public async void DeleteGroup_NonExistent_False()
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

            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(null, false, new[] {new Error("404", "Group Not Found")}));

            // Use Case and OutputPort

            DeleteGroupUseCase useCase =
                new DeleteGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteGroupResponse> mockOutputPort = new MockOutputPort<DeleteGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
        }

        [Fact]
        public async void DeleteGroup_NotInOrganisation_False()
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

            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            DeleteGroupUseCase useCase =
                new DeleteGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteGroupResponse> mockOutputPort = new MockOutputPort<DeleteGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
        }

        [Fact]
        public async void DeleteGroup_RepoDeleteErrors_False()
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

            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Group Delete Failed")}));
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            DeleteGroupUseCase useCase =
                new DeleteGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteGroupResponse> mockOutputPort = new MockOutputPort<DeleteGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
        }
    }
}