using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.Services;
using Api.Core.UseCases.Invite;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.InviteTests
{
    public class CreateTests
    {
        [Fact]
        public async Task CreateInvite_NonOrgUser_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Id = orgId
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            Invite mockInvite = new Invite
            {
                OrganisationId = orgId,
                Email = inviteEmailAddress,
                Token = inviteToken
            };

            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Create(It.IsAny<Invite>()))
                .ReturnsAsync(new CreateInviteGatewayResponse(mockInvite, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(inviteToken);

            // Use Case and OutputPort

            CreateInviteUseCase useCase = new CreateInviteUseCase(mockOrganisationRepository.Object,
                mockInviteRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateInviteResponse> mockOutputPort = new MockOutputPort<CreateInviteResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateInviteRequest(Guid.NewGuid().ToString(), orgId, inviteEmailAddress),
                    mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.EditClients)]
        [InlineData(OrganisationPermissions.EditGroups)]
        [InlineData(OrganisationPermissions.EditOrgUsers)]
        [InlineData(OrganisationPermissions.EditOrg)]
        [InlineData(OrganisationPermissions.CreateClients)]
        [InlineData(OrganisationPermissions.CreateGroups)]
        [InlineData(OrganisationPermissions.DeleteClients)]
        [InlineData(OrganisationPermissions.DeleteGroups)]
        [InlineData(OrganisationPermissions.DeleteOrgUsers)]
        public async Task CreateInvite_WrongPermission_False(OrganisationPermissions permission)
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

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

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            Invite mockInvite = new Invite
            {
                OrganisationId = orgId,
                Email = inviteEmailAddress,
                Token = inviteToken
            };

            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Create(It.IsAny<Invite>()))
                .ReturnsAsync(new CreateInviteGatewayResponse(mockInvite, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(inviteToken);

            // Use Case and OutputPort

            CreateInviteUseCase useCase = new CreateInviteUseCase(mockOrganisationRepository.Object,
                mockInviteRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateInviteResponse> mockOutputPort = new MockOutputPort<CreateInviteResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateInviteRequest(userId.ToString(), orgId, inviteEmailAddress),
                    mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Theory]
        [InlineData(OrganisationPermissions.CreateOrgUsers)]
        [InlineData(OrganisationPermissions.All)]
        public async Task CreateInvite_CorrectPermission_True(OrganisationPermissions permission)
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

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

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            Invite mockInvite = new Invite
            {
                OrganisationId = orgId,
                Email = inviteEmailAddress,
                Token = inviteToken
            };

            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Create(It.IsAny<Invite>()))
                .ReturnsAsync(new CreateInviteGatewayResponse(mockInvite, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(inviteToken);

            // Use Case and OutputPort

            CreateInviteUseCase useCase = new CreateInviteUseCase(mockOrganisationRepository.Object,
                mockInviteRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateInviteResponse> mockOutputPort = new MockOutputPort<CreateInviteResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateInviteRequest(userId.ToString(), orgId, inviteEmailAddress),
                    mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async Task CreateInvite_OrgOwner_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = userId.ToString(),
                Users = new[] {new OrganisationUser {Id = userId.ToString()}}
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            Invite mockInvite = new Invite
            {
                OrganisationId = orgId,
                Email = inviteEmailAddress,
                Token = inviteToken
            };

            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Create(It.IsAny<Invite>()))
                .ReturnsAsync(new CreateInviteGatewayResponse(mockInvite, true));

            Mock<IPasswordGenerator> mockPasswordGenerator = new Mock<IPasswordGenerator>();
            mockPasswordGenerator
                .Setup(repo => repo.Generate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(inviteToken);

            // Use Case and OutputPort

            CreateInviteUseCase useCase = new CreateInviteUseCase(mockOrganisationRepository.Object,
                mockInviteRepository.Object, mockPasswordGenerator.Object);

            MockOutputPort<CreateInviteResponse> mockOutputPort = new MockOutputPort<CreateInviteResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateInviteRequest(userId.ToString(), orgId, inviteEmailAddress),
                    mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}