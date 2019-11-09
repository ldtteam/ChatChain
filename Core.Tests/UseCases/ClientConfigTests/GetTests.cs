using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.ClientConfig;
using Api.Core.DTO.UseCaseResponses.ClientConfig;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.ClientConfig;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.UseCases.ClientConfigTests
{
    public class GetTests
    {
        [Fact]
        public async Task GetClientConfig_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //ClientConfig Arrange

            Guid configId = Guid.NewGuid();

            //For the first "control" test we'll set all the variables and assert them down below.
            IEnumerable<Guid> clientEventGroups = new List<Guid> {Guid.NewGuid()};
            IEnumerable<Guid> userEventGroups = new List<Guid> {Guid.NewGuid()};

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = configId,
                OwnerId = orgId,
                ClientEventGroups = clientEventGroups,
                UserEventGroups = userEventGroups
            };

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            MockOutputPort<GetClientConfigResponse> mockOutputPort = new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientConfigRequest(orgId, configId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.ClientConfig.Id, configId);

            // Assert ClientConfig Values
            Assert.Equal(mockOutputPort.Response.ClientConfig.OwnerId, orgId);
            Assert.Equal(mockOutputPort.Response.ClientConfig.ClientEventGroups, clientEventGroups);
            Assert.Equal(mockOutputPort.Response.ClientConfig.UserEventGroups, userEventGroups);
        }

        [Fact]
        public async Task GetClientConfig_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            //ClientConfig Arrange

            Guid configId = Guid.NewGuid();

            ClientConfig mockClientConfig = new ClientConfig {Id = configId, OwnerId = orgId};

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            MockOutputPort<GetClientConfigResponse> mockOutputPort = new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientConfigRequest(orgId, configId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetClientConfig_NonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //ClientConfig Arrange

            Guid configId = Guid.NewGuid();

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(null, false,
                    new[] {new Error("404", "Client Config Not Found")}));

            // Use Case and OutputPort

            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            MockOutputPort<GetClientConfigResponse> mockOutputPort = new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientConfigRequest(orgId, configId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetClientConfig_NotInOrganisation_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //ClientConfig Arrange

            Guid configId = Guid.NewGuid();

            ClientConfig mockClientConfig = new ClientConfig {Id = configId, OwnerId = Guid.NewGuid()};

            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            MockOutputPort<GetClientConfigResponse> mockOutputPort = new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetClientConfigRequest(orgId, configId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}