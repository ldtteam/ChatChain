using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.Organisation;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.UseCases.OrganisationTests
{
    public class DeleteTests
    {
        [Fact]
        public async void DeleteOrganisation_True()
        {
            // Arrange \\

            // Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();


            // Act \\
            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);


            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.Message);

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_NonExistent_False()
        {
            // Arrange \\

            Guid organisationId = Guid.NewGuid();

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(deleteRan);
            Assert.False(clientConfigDeleteRan);
            Assert.False(is4ClientDeleteRan);
            Assert.False(clientDeleteRan);
            Assert.False(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_OrganisationRepoDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Organisation Delete Failed")}));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));            
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.False(clientConfigDeleteRan);
            Assert.False(is4ClientDeleteRan);
            Assert.False(clientDeleteRan);
            Assert.False(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_ClientConfigRepoDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Config Delete Failed")}));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }
        
        [Fact]
        public async void DeleteOrganisation_IS4ClientRepoDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "IS4Client Delete Failed")}));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("IS4Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_ClientRepoDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Delete Failed")}));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_GroupRepoDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Group Delete Failed")}));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Group Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }

        [Fact]
        public async void DeleteOrganisation_ClientConfigAndClientAndGroupReposDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Delete Values
            Guid ownerId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            IList<OrganisationUser> users = new List<OrganisationUser> {new OrganisationUser()};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = organisationId,
                Owner = ownerId.ToString(),
                Name = organisationName,
                Users = users
            };

            bool deleteRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Delete(It.Is<Guid>(id => id.Equals(organisationId))))
                .Callback(() => deleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            bool clientConfigDeleteRan = false;
            Mock<IClientConfigRepository> mockClientConfigRepository = new Mock<IClientConfigRepository>();
            mockClientConfigRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientConfigDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Config Delete Failed")}));

            bool is4ClientDeleteRan = false;
            Mock<IIS4ClientRepository> mockIS4ClientRepository = new Mock<IIS4ClientRepository>();
            mockIS4ClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => is4ClientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "IS4Client Delete Failed")}));
            
            bool clientDeleteRan = false;
            Mock<IClientRepository> mockClientRepository = new Mock<IClientRepository>();
            mockClientRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => clientDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Client Delete Failed")}));

            bool groupDeleteRan = false;
            Mock<IGroupRepository> mockGroupRepository = new Mock<IGroupRepository>();
            mockGroupRepository
                .Setup(repo => repo.DeleteForOwner(It.IsAny<Guid>()))
                .Callback(() => groupDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Group Delete Failed")}));

            // The Use Case we are testing (DeleteOrganisation)
            DeleteOrganisationUseCase useCase = new DeleteOrganisationUseCase(mockOrganisationRepository.Object,
                mockClientConfigRepository.Object, mockClientRepository.Object, mockIS4ClientRepository.Object, mockGroupRepository.Object);

            MockOutputPort<DeleteOrganisationResponse>
                mockOutputPort = new MockOutputPort<DeleteOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new DeleteOrganisationRequest(organisationId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Client Config Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
            Assert.Contains("IS4Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
            Assert.Contains("Client Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
            Assert.Contains("Group Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(deleteRan);
            Assert.True(clientConfigDeleteRan);
            Assert.True(is4ClientDeleteRan);
            Assert.True(clientDeleteRan);
            Assert.True(groupDeleteRan);
        }
    }
}