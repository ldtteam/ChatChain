using System;
using System.Collections.Generic;
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
    public class GetsTests
    {
        [Fact]
        public async Task GetClients_True()
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
                .Setup(repo => repo.GetForOwner(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetClientsGatewayResponse(new List<Client> {mockClient}, true));

            // Use Case and OutputPort

            GetClientsUseCase useCase =
                new GetClientsUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientsResponse> mockOutputPort = new MockOutputPort<GetClientsResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientsRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Contains(mockClient, mockOutputPort.Response.Clients);
        }

        [Fact]
        public async Task GetClients_OrganisationNonExistent_False()
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
                .Setup(repo => repo.GetForOwner(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetClientsGatewayResponse(new List<Client> {mockClient}, true));

            // Use Case and OutputPort

            GetClientsUseCase useCase =
                new GetClientsUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientsResponse> mockOutputPort = new MockOutputPort<GetClientsResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientsRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetClients_RepoGetErrors_False()
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

            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.GetForOwner(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(
                    new GetClientsGatewayResponse(null, false, new[] {new Error("500", "Clients Get Failed")}));

            // Use Case and OutputPort

            GetClientsUseCase useCase =
                new GetClientsUseCase(mockOrganisationRepository.Object, mockClientRepository.Object);

            MockOutputPort<GetClientsResponse> mockOutputPort = new MockOutputPort<GetClientsResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientsRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Clients Get Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}