using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core.DTO;
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

namespace Api.Tests.UseCases.ClientTests
{
    public class CreateTests
    {
        [Fact]
        public async void CreateClient_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);

            IEnumerable<Guid> clientEventGroups = new List<Guid> {Guid.NewGuid()};
            IEnumerable<Guid> userEventGroups = new List<Guid> {Guid.NewGuid()};

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId,
                ClientEventGroups = clientEventGroups,
                UserEventGroups = userEventGroups
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateClientRequest(orgId, clientName, clientDescription),
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
            Assert.Equal(mockOutputPort.Response.Client.OwnerId, orgId);
            Assert.Equal(mockOutputPort.Response.ClientConfig.OwnerId, orgId);

            // Assert Password Value
            Assert.Equal(mockOutputPort.Response.Password, password);

            // Assert Client Values
            Assert.Equal(mockOutputPort.Response.Client.Id, clientId);
            Assert.Equal(mockOutputPort.Response.Client.Name, clientName);
            Assert.Equal(mockOutputPort.Response.Client.Description, clientDescription);

            // Assert Client Config Values
            Assert.Equal(mockOutputPort.Response.ClientConfig.Id, clientId);
            Assert.Equal(mockOutputPort.Response.ClientConfig.ClientEventGroups, clientEventGroups);
            Assert.Equal(mockOutputPort.Response.ClientConfig.UserEventGroups, userEventGroups);
        }

        [Fact]
        public async void CreateClient_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

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

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateClientRequest(orgId, clientName, clientDescription),
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
        public async void CreateClient_IS4ClientRepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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
                .ReturnsAsync(new CreateIS4ClientGatewayResponse(false, new[] {new Error("500", "IS4Client Create Failed")}));

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

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateClientRequest(orgId, clientName, clientDescription),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("IS4Client Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async void CreateClient_ClientRepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Create(It.IsAny<Client>()))
                .ReturnsAsync(new CreateClientGatewayResponse(null, false,
                    new[] {new Error("500", "Client Create Failed")}));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(mockClientConfig, true));
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateClientRequest(orgId, clientName, clientDescription),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async void CreateClient_ClientConfigRepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(password);

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Create(It.IsAny<ClientConfig>()))
                .ReturnsAsync(new CreateClientConfigGatewayResponse(null, false,
                    new[] {new Error("500", "Client Config Create Failed")}));
            // Use Case and OutputPort

            CreateClientUseCase useCase = new CreateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateClientResponse> mockOutputPort = new MockOutputPort<CreateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateClientRequest(orgId, clientName, clientDescription),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}