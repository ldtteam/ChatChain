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
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateClientConfig_True()
        {
            // Arrange \\

            // Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // ClientConfig Arrange

            Guid configId = Guid.NewGuid();
            IEnumerable<Guid> clientEventGroups = new List<Guid> {Guid.NewGuid()};
            IEnumerable<Guid> userEventGroups = new List<Guid> {Guid.NewGuid()};

            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = configId,
                OwnerId = orgId,
                ClientEventGroups = clientEventGroups,
                UserEventGroups = userEventGroups
            };

            bool updateRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(configId)), It.IsAny<ClientConfig>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientConfigGatewayResponse(mockClientConfig, true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            UpdateClientConfigUseCase useCase = new UpdateClientConfigUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object);

            MockOutputPort<UpdateClientConfigResponse>
                mockOutputPort = new MockOutputPort<UpdateClientConfigResponse>();


            // Act \\
            bool response = await useCase.HandleAsync(
                new UpdateClientConfigRequest(orgId, configId, clientEventGroups, userEventGroups), mockOutputPort);


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
            Assert.Equal(mockOutputPort.Response.ClientConfig.Id, configId);

            // Assert Updated Values.
            Assert.Equal(mockOutputPort.Response.ClientConfig.ClientEventGroups, clientEventGroups);
            Assert.Equal(mockOutputPort.Response.ClientConfig.UserEventGroups, userEventGroups);
        }

        [Fact]
        public async Task UpdateClientConfig_OrganisationNonExistent_False()
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

            bool updateRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(configId)), It.IsAny<ClientConfig>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientConfigGatewayResponse(mockClientConfig, true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            UpdateClientConfigUseCase useCase = new UpdateClientConfigUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object);

            MockOutputPort<UpdateClientConfigResponse>
                mockOutputPort = new MockOutputPort<UpdateClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateClientConfigRequest(orgId, configId, new List<Guid>(), new List<Guid>()), mockOutputPort);

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
        public async Task UpdateClientConfig_NonExistent_False()
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

            ClientConfig mockClientConfig = new ClientConfig {Id = configId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(configId)), It.IsAny<ClientConfig>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientConfigGatewayResponse(mockClientConfig, true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(null, false,
                    new[] {new Error("404", "Client Config Not Found")}));

            // Use Case and OutputPort

            UpdateClientConfigUseCase useCase = new UpdateClientConfigUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object);

            MockOutputPort<UpdateClientConfigResponse>
                mockOutputPort = new MockOutputPort<UpdateClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateClientConfigRequest(orgId, configId, new List<Guid>(), new List<Guid>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateClientConfig_NotInOrganisation_False()
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

            bool updateRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(configId)), It.IsAny<ClientConfig>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientConfigGatewayResponse(mockClientConfig, true));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            UpdateClientConfigUseCase useCase = new UpdateClientConfigUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object);

            MockOutputPort<UpdateClientConfigResponse>
                mockOutputPort = new MockOutputPort<UpdateClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateClientConfigRequest(orgId, configId, new List<Guid>(), new List<Guid>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateClientConfig_RepoUpdateErrors_False()
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

            ClientConfig mockClientConfig = new ClientConfig {Id = configId, OwnerId = orgId};

            bool updateRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(configId)), It.IsAny<ClientConfig>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateClientConfigGatewayResponse(null, false,
                    new[] {new Error("500", "Client Config Update Failed")}));
            mockClientConfigRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(configId))))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // Use Case and OutputPort

            UpdateClientConfigUseCase useCase = new UpdateClientConfigUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object);

            MockOutputPort<UpdateClientConfigResponse>
                mockOutputPort = new MockOutputPort<UpdateClientConfigResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateClientConfigRequest(orgId, configId, new List<Guid>(), new List<Guid>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(updateRan);
        }
    }
}