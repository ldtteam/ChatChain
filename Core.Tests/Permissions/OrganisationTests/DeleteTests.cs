using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.Organisation;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.OrganisationTests
{
    public class DeleteTests
    {
        [Fact]
        public async Task DeleteOrganisation_NonOrgUser_False()
        {
            // Arrange \\

            // GUID of org we're testing with, 
            Guid orgId = Guid.NewGuid();
            Guid ownerId = Guid.NewGuid();

            // Mock Organisation with ownerId set to OrgId
            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = ownerId.ToString()
            };

            // Mock Organisation Repo that returns mockOrganisation
            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteOrganisationResponse> mockOutputPort =
                new MockOutputPort<DeleteOrganisationResponse>();

            // GUID of the anonymous user we're testing with
            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteOrganisationRequest(userId.ToString(), Guid.NewGuid()), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
            Assert.False(clientConfigDeleteRan);
            Assert.False(is4ClientDeleteRan);
            Assert.False(clientDeleteRan);
            Assert.False(groupDeleteRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        [InlineData(OrganisationPermissions.All)]
        public async Task DeleteOrganisation_WrongPermission_False(OrganisationPermissions permission)
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
            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteOrganisationResponse> mockOutputPort =
                new MockOutputPort<DeleteOrganisationResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteOrganisationRequest(userId.ToString(), Guid.NewGuid()), mockOutputPort);

            // Assert
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
            Assert.False(clientConfigDeleteRan);
            Assert.False(is4ClientDeleteRan);
            Assert.False(clientDeleteRan);
            Assert.False(groupDeleteRan);
        }

        [Fact]
        public async Task DeleteOrganisation_OrgOwner_True()
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
            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteOrganisationResponse> mockOutputPort =
                new MockOutputPort<DeleteOrganisationResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteOrganisationRequest(userId.ToString(), Guid.NewGuid()), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }
    }
}