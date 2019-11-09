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
    public class GetTests
    {
        [Fact]
        public async Task GetGroup_NonOrgUser_False()
        {
            // Arrange \\

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
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
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (UpdateGroup)
            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetGroupResponse> mockOutputPort =
                new MockOutputPort<GetGroupResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new GetGroupRequest(Guid.NewGuid().ToString(), Guid.NewGuid(), Guid.NewGuid()), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetGroup_OrgUser_True()
        {
            // Arrange \\

            Guid userId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Id = orgId,
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = userId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
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
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (UpdateGroup)
            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetGroupResponse> mockOutputPort =
                new MockOutputPort<GetGroupResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response =
                await useCase.HandleAsync(new GetGroupRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid()),
                    mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async Task GetGroup_OrgOwner_True()
        {
            // Arrange \\

            Guid userId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = userId.ToString(),
                Id = orgId,
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
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetGroupGatewayResponse(mockGroup, true));

            // The Use Case we are testing (UpdateGroup)
            GetGroupUseCase useCase =
                new GetGroupUseCase(mockOrganisationRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetGroupResponse> mockOutputPort =
                new MockOutputPort<GetGroupResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response =
                await useCase.HandleAsync(new GetGroupRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid()),
                    mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}