using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.OrganisationUser;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.OrganisationUserTests
{
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateOrganisationUser_NonOrgUser_False()
        {
            // Arrange \\

            // GUID of org we're testing with, 
            Guid orgId = Guid.NewGuid();

            Guid updateUserId = Guid.NewGuid();

            // Mock organisation with ID set
            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = new List<OrganisationUser> {new OrganisationUser {Id = updateUserId.ToString()}}
            };

            // Mock Organisation Repo that returns mockOrganisation
            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateOrganisationUser)
            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // GUID of the anonymous user we're testing with
            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), updateUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation User Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task UpdateOrganisationUser_WrongPermission_False(OrganisationPermissions permission)
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            Guid updateUserId = Guid.NewGuid();

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
                    },
                    new OrganisationUser
                    {
                        Id = updateUserId.ToString()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateOrganisationUser)
            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), updateUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation User Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.All)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        public async Task UpdateOrganisationUser_CorrectPermission_True(OrganisationPermissions permission)
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            Guid updateUserId = Guid.NewGuid();

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
                    },
                    new OrganisationUser
                    {
                        Id = updateUserId.ToString()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateOrganisationUser)
            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), updateUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(updateRan);
        }

        [Fact]
        public async Task UpdateOrganisationUser_OrgOwner_True()
        {
            // Arrange

            // GUID of org we're testing with
            Guid orgId = Guid.NewGuid();

            // GUID of user we're testing with
            Guid userId = Guid.NewGuid();

            Guid updateUserId = Guid.NewGuid();

            // Mock organisation with Owner set as UserID
            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = userId.ToString(),
                Users = new[]
                    {new OrganisationUser {Id = userId.ToString()}, new OrganisationUser {Id = updateUserId.ToString()}}
            };

            // Mock Organisation Repo that returns mockOrganisation
            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateOrganisationUser)
            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), updateUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(updateRan);
        }
    }
}