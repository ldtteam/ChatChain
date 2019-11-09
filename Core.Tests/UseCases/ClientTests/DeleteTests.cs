using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
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

namespace Api.Tests.UseCases.ClientTests
{
    public class DeleteTests
    {
        [Fact]
        public async Task DeleteClient_True()
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

            Guid clientId = Guid.NewGuid();

            //For the first "control" test we'll set all the variables and assert them down below.
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";

            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);

            Assert.True(is4DeleteRan);
            Assert.True(deleteRan);
            Assert.True(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_OrganisationNonExistent_False()
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
            
            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(is4DeleteRan);
            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_NonExistent_False()
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
            
            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            Guid clientId = Guid.NewGuid();

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(null, false, new[] {new Error("404", "Client Not Found")}));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(is4DeleteRan);
            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_NotInOrganisation_False()
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
            
            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = Guid.NewGuid()};

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(is4DeleteRan);
            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }
        
        [Fact]
        public async Task DeleteClient_IS4ClientRepoDeleteErrors_False()
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

            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "IS4Client Delete Failed")}));

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("IS4Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(is4DeleteRan);
            Assert.False(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_ClientRepoDeleteErrors_False()
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
            
            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Delete Failed")}));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(is4DeleteRan);
            Assert.True(deleteRan);
            Assert.False(configDeleteRan);
        }

        [Fact]
        public async Task DeleteClient_ClientConfigRepoDeleteErrors_False()
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
            
            bool is4DeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => is4DeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool deleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = clientId,
                OwnerId = orgId
            };

            bool configDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>()))
                .Callback(() => configDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Config Delete Failed")}));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            DeleteClientUseCase useCase = new DeleteClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object, mockClientConfigRepository.Object, mockIS4ClientRepository.Object);

            MockOutputPort<DeleteClientResponse> mockOutputPort = new MockOutputPort<DeleteClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(is4DeleteRan);
            Assert.True(deleteRan);
            Assert.True(configDeleteRan);
        }
    }
}