using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Api.Tests.Permissions.ClientTests
{
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateClient_NonOrgUser_False()
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

            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo =>
                    repo.Update(It.IsAny<Guid>(), It.IsAny<Client>()))
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // The Use Case we are testing (UpdateClient)
            UpdateClientUseCase useCase = new UpdateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateClientResponse> mockOutputPort =
                new MockOutputPort<UpdateClientResponse>();

            // GUID of the anonymous user we're testing with
            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateClientRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid(), "", ""), mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task UpdateClient_WrongPermission_False(OrganisationPermissions permission)
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

            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo =>
                    repo.Update(It.IsAny<Guid>(), It.IsAny<Client>()))
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // The Use Case we are testing (UpdateClient)
            UpdateClientUseCase useCase = new UpdateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateClientResponse> mockOutputPort =
                new MockOutputPort<UpdateClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateClientRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid(), "", ""), mockOutputPort);

            // Assert
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.All)]
        public async Task UpdateClient_CorrectPermission_True(OrganisationPermissions permission)
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

            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<Client>()))
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // The Use Case we are testing (UpdateClient)
            UpdateClientUseCase useCase = new UpdateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateClientResponse> mockOutputPort =
                new MockOutputPort<UpdateClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateClientRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid(), "", ""), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async Task UpdateClient_OrgOwner_True()
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

            // Mock Client with ownerId set to OrgId
            Client mockClient = new Client
            {
                OwnerId = orgId
            };

            // Mock Client Repo that returns mockClient
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo =>
                    repo.Update(It.IsAny<Guid>(), It.IsAny<Client>()))
                .ReturnsAsync(new UpdateClientGatewayResponse(mockClient, true));
            mockClientRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientGatewayResponse(mockClient, true));

            // The Use Case we are testing (UpdateClient)
            UpdateClientUseCase useCase = new UpdateClientUseCase(mockOrganisationRepository.Object,
                mockClientRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<UpdateClientResponse> mockOutputPort =
                new MockOutputPort<UpdateClientResponse>();

            // Act

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new UpdateClientRequest(userId.ToString(), Guid.NewGuid(), Guid.NewGuid(), "", ""), mockOutputPort);

            // Assert
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}