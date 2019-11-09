using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
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
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateClient_True()
        {
            // Arrange \\

            // Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Client Arrange

            Guid clientId = Guid.NewGuid();

            // Update Values
            const string clientName = "Test Client";
            const string clientDescription = "Test Description";

            Client mockClient = new Client
                {Id = clientId, OwnerId = orgId, Name = clientName, Description = clientDescription};

            bool updateRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(clientId)), It.IsAny<Client>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            UpdateClientUseCase useCase =
                new UpdateClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<UpdateClientResponse> mockOutputPort = new MockOutputPort<UpdateClientResponse>();


            // Act \\
            bool response =
                await useCase.HandleAsync(new UpdateClientRequest(orgId, clientId, clientName, clientDescription),
                    mockOutputPort);


            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.Message);

            Assert.True(updateRan);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Client.Id, clientId);

            // Assert Updated Values.
            Assert.Equal(mockOutputPort.Response.Client.Name, clientName);
            Assert.Equal(mockOutputPort.Response.Client.Description, clientDescription);
        }

        [Fact]
        public async Task UpdateClient_OrganisationNonExistent_False()
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

            Guid clientId = Guid.NewGuid();

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(clientId)), It.IsAny<Client>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            UpdateClientUseCase useCase =
                new UpdateClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<UpdateClientResponse> mockOutputPort = new MockOutputPort<UpdateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateClientRequest(orgId, clientId, "", ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateClient_NonExistent_False()
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

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(clientId)), It.IsAny<Client>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(null, false, new[] {new Error("404", "Client Not Found")}));

            // Use Case and OutputPort

            UpdateClientUseCase useCase =
                new UpdateClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<UpdateClientResponse> mockOutputPort = new MockOutputPort<UpdateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateClientRequest(orgId, clientId, "", ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateClient_NotInOrganisation_False()
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

            Client mockClient = new Client {Id = clientId, OwnerId = Guid.NewGuid()};

            bool updateRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(clientId)), It.IsAny<Client>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            UpdateClientUseCase useCase =
                new UpdateClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<UpdateClientResponse> mockOutputPort = new MockOutputPort<UpdateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateClientRequest(orgId, clientId, "", ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateClient_RepoUpdateErrors_False()
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

            Client mockClient = new Client {Id = clientId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(clientId)), It.IsAny<Client>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientGatewayResponse(null, false,
                    new[] {new Error("500", "Client Update Failed")}));
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            UpdateClientUseCase useCase =
                new UpdateClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<UpdateClientResponse> mockOutputPort = new MockOutputPort<UpdateClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UpdateClientRequest(orgId, clientId, "", ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(updateRan);
        }
    }
}