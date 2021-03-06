using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Api.Tests.Permissions.GroupTests
{
    public class DeleteTests
    {
        [Fact]
        public async Task DeleteGroup_NonOrgUser_False()
        {
            // Arrange \\

            // GUID of org we're testing with, 
            Guid orgId = Guid.NewGuid();

            // Mock organisation with ID set
            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Mock Group with ownerId set to OrgId
            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Group Repo that returns mockGroup
            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (DeleteGroup)
            DeleteGroupUseCase useCase = new DeleteGroupUseCase(mockOrganisationRepository.Object,
                mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteGroupResponse> mockOutputPort =
                new MockOutputPort<DeleteGroupResponse>();

            // GUID of the anonymous user we're testing with
            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteGroupRequest(userId.ToString(), Guid.NewGuid(), mockGroup.Id), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task DeleteGroup_WrongPermission_False(OrganisationPermissions permission)
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            // Mock organisation with User set to userId with No permissions
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

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Mock Group with ownerId set to OrgId
            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Group Repo that returns mockGroup
            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (DeleteGroup)
            DeleteGroupUseCase useCase = new DeleteGroupUseCase(mockOrganisationRepository.Object,
                mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteGroupResponse> mockOutputPort =
                new MockOutputPort<DeleteGroupResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteGroupRequest(userId.ToString(), Guid.NewGuid(), mockGroup.Id), mockOutputPort);

            // Assert
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.All)]
        public async Task DeleteGroup_CorrectPermission_True(OrganisationPermissions permission)
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            // Mock organisation with User set to userId with No permissions
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

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Mock Group with ownerId set to OrgId
            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Group Repo that returns mockGroup
            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (DeleteGroup)
            DeleteGroupUseCase useCase = new DeleteGroupUseCase(mockOrganisationRepository.Object,
                mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteGroupResponse> mockOutputPort =
                new MockOutputPort<DeleteGroupResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteGroupRequest(userId.ToString(), Guid.NewGuid(), mockGroup.Id), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(deleteRan);
        }

        [Fact]
        public async Task DeleteGroup_OrgOwner_True()
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            // Mock organisation with Owner set as UserID
            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = userId.ToString(),
                Users = new[] {new OrganisationUser {Id = userId.ToString()}}
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Mock Group with ownerId set to OrgId
            Group mockGroup = new Group
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Group Repo that returns mockGroup
            bool deleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockGroupRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (DeleteGroup)
            DeleteGroupUseCase useCase = new DeleteGroupUseCase(mockOrganisationRepository.Object,
                mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteGroupResponse> mockOutputPort =
                new MockOutputPort<DeleteGroupResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteGroupRequest(userId.ToString(), Guid.NewGuid(), mockGroup.Id), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(deleteRan);
        }
    }
}