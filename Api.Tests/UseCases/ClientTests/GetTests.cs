using System;
using System.Linq;
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
    public class GetTests
    {
        [Fact]
        public async void GetClient_True()
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

            Client mockClient = new Client
            {
                Id = clientId,
                OwnerId = orgId,
                Name = clientName,
                Description = clientDescription
            };

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            GetClientUseCase useCase =
                new GetClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientResponse> mockOutputPort = new MockOutputPort<GetClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Client.Id, clientId);

            // Assert Client Values
            Assert.Equal(mockOutputPort.Response.Client.OwnerId, orgId);
            Assert.Equal(mockOutputPort.Response.Client.Name, clientName);
            Assert.Equal(mockOutputPort.Response.Client.Description, clientDescription);
        }

        [Fact]
        public async void GetClient_OrganisationNonExistent_False()
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

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            GetClientUseCase useCase =
                new GetClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientResponse> mockOutputPort = new MockOutputPort<GetClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async void GetClient_NonExistent_False()
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

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(null, false, new[] {new Error("404", "Client Not Found")}));

            // Use Case and OutputPort

            GetClientUseCase useCase =
                new GetClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientResponse> mockOutputPort = new MockOutputPort<GetClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async void GetClient_NotInOrganisation_False()
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

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(clientId))))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // Use Case and OutputPort

            GetClientUseCase useCase =
                new GetClientUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientResponse> mockOutputPort = new MockOutputPort<GetClientResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientRequest(orgId, clientId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}