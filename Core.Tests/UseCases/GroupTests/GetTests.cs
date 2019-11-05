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
    public class GetTests
    {
        [Fact]
        public async Task GetGroup_True()
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
            List<Guid> clientIds = new List<Guid> {Guid.NewGuid()};

            Group mockGroup = new Group
            {
                Id = groupId,
                OwnerId = orgId,
                Name = groupName,
                Description = groupDescription,
                ClientIds = clientIds
            };

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<GetGroupResponse> mockOutputPort = new MockOutputPort<GetGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Group.Id, groupId);

            // Assert Group Values
            Assert.Equal(mockOutputPort.Response.Group.OwnerId, orgId);
            Assert.Equal(mockOutputPort.Response.Group.Name, groupName);
            Assert.Equal(mockOutputPort.Response.Group.Description, groupDescription);
            Assert.Equal(mockOutputPort.Response.Group.ClientIds, clientIds);
        }

        [Fact]
        public async Task GetGroup_OrganisationNonExistent_False()
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

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<GetGroupResponse> mockOutputPort = new MockOutputPort<GetGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetGroup_NonExistent_False()
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

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(null, false, new[] {new Error("404", "Group Not Found")}));

            // Use Case and OutputPort

            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<GetGroupResponse> mockOutputPort = new MockOutputPort<GetGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetGroup_NotInOrganisation_False()
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

            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(groupId))))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // Use Case and OutputPort

            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            MockOutputPort<GetGroupResponse> mockOutputPort = new MockOutputPort<GetGroupResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetGroupRequest(orgId, groupId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}