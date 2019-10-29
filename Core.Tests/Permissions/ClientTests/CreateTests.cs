using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.IS4Client;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.Services;
using Api.Core.UseCases.Client;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.ClientTests
{
    public class CreateTests
    {
        [Fact]
        public async void CreateClient_NonOrgUser_False()
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

            //Client Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";
            const string password = "123";
            
            Guid clientId = Guid.NewGuid();

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Create(It.IsAny<IS4Client>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateIS4ClientGatewayResponse(true));

            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Create(It.IsAny<Client>()))
                .ReturnsAsync(new CreateClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));
            
            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateClientRequest(Guid.NewGuid().ToString(), orgId, clientName, clientDescription),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        [InlineData(OrganisationPermissions.EditClients)]
        public async void CreateClient_WrongPermission_False(OrganisationPermissions permission)
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

            //Client Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";
            const string password = "123";
            
            Guid clientId = Guid.NewGuid();

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Create(It.IsAny<IS4Client>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateIS4ClientGatewayResponse(true));
            
            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Create(It.IsAny<Client>()))
                .ReturnsAsync(new CreateClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateClientRequest(userId.ToString(), orgId, clientName, clientDescription), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.All)]
        public async void CreateClient_CorrectPermission_True(OrganisationPermissions permission)
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

            //Client Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";
            const string password = "123";
            
            Guid clientId = Guid.NewGuid();

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Create(It.IsAny<IS4Client>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateIS4ClientGatewayResponse(true));

            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Create(It.IsAny<Client>()))
                .ReturnsAsync(new CreateClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateClientRequest(userId.ToString(), orgId, clientName, clientDescription), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async void CreateClient_OrgOwner_True()
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

            //Client Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";
            const string password = "123";
            
            Guid clientId = Guid.NewGuid();

            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Create(It.IsAny<IS4Client>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateIS4ClientGatewayResponse(true));

            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Create(It.IsAny<Client>()))
                .ReturnsAsync(new CreateClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new CreateClientRequest(userId.ToString(), orgId, clientName, clientDescription), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}