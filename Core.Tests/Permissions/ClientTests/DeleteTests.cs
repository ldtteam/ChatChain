using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.Client;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.ClientTests
{
    public class DeleteTests
    {
        [Fact]
        public async Task DeleteClient_NonOrgUser_False()
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

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteClientResponse> mockOutputPort =
                new MockOutputPort<DeleteClientResponse>();

            // GUID of the anonymous user we're testing with
            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteClientRequest(userId.ToString(), Guid.NewGuid(), mockClient.Id), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task DeleteClient_WrongPermission_False(OrganisationPermissions permission)
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

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteClientResponse> mockOutputPort =
                new MockOutputPort<DeleteClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteClientRequest(userId.ToString(), Guid.NewGuid(), mockClient.Id), mockOutputPort);

            // Assert
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Theory]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.All)]
        public async Task DeleteClient_CorrectPermission_True(OrganisationPermissions permission)
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

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteClientResponse> mockOutputPort =
                new MockOutputPort<DeleteClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteClientRequest(userId.ToString(), Guid.NewGuid(), mockClient.Id), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(deleteRan);
            Assert.True(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_OrgOwner_True()
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

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = Guid.NewGuid(),
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<DeleteClientResponse> mockOutputPort =
                new MockOutputPort<DeleteClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new DeleteClientRequest(userId.ToString(), Guid.NewGuid(), mockClient.Id), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(deleteRan);
            Assert.True(configDeleteRan);
        }
    }
}