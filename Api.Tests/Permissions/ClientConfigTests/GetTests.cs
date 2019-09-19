using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Api.Tests.Permissions.ClientConfigTests
{
    public class GetTests
    {
        [Fact]
        public async void GetClientConfig_NonOrgUser_False()
        {
            // Arrange \\

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Id = orgId
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            Guid configId = Guid.NewGuid();

            // Mock ClientConfig with ownerId set to OrgId
            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = configId,
                OwnerId = orgId
            };

            // Mock ClientConfig Repo that returns mockClientConfig
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetClientConfigResponse> mockOutputPort =
                new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new GetClientConfigRequest(Guid.NewGuid().ToString(), Guid.NewGuid(), Guid.NewGuid()), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async void GetClientConfig_OrgUser_True()
        {
            // Arrange \\

            Guid userId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Id = orgId,
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = userId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            Guid configId = Guid.NewGuid();

            // Mock ClientConfig with ownerId set to OrgId
            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = configId,
                OwnerId = orgId
            };

            // Mock ClientConfig Repo that returns mockClientConfig
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetClientConfigResponse> mockOutputPort =
                new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response =
                await useCase.HandleAsync(new GetClientConfigRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid()),
                    mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async void GetClientConfig_OrgOwner_True()
        {
            // Arrange \\

            Guid userId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = userId.ToString(),
                Id = orgId,
                Users = new[] {new OrganisationUser {Id = userId.ToString()}}
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            Guid configId = Guid.NewGuid();

            // Mock ClientConfig with ownerId set to OrgId
            ClientConfig mockClientConfig = new ClientConfig
            {
                Id = configId,
                OwnerId = orgId
            };

            // Mock ClientConfig Repo that returns mockClientConfig
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo =>
                    repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientConfigGatewayResponse(mockClientConfig, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetClientConfigUseCase useCase =
                new GetClientConfigUseCase(mockOrganisationRepository.Object, mockClientConfigRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetClientConfigResponse> mockOutputPort =
                new MockOutputPort<GetClientConfigResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response =
                await useCase.HandleAsync(new GetClientConfigRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid()),
                    mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}