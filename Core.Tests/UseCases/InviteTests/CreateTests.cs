using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
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

namespace Api.Tests.UseCases.InviteTests
{
    public class CreateTests
    {
        [Fact]
        public async Task CreateInvite_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

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
                await useCase.HandleAsync(new CreateInviteRequest(orgId, inviteEmailAddress), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Invite.OrganisationId, orgId);

            // Assert Invite Values
            Assert.Equal(mockOutputPort.Response.Invite.Email, inviteEmailAddress);
            Assert.Equal(mockOutputPort.Response.Invite.Token, inviteToken);
        }

        [Fact]
        public async Task CreateInvite_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

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
                await useCase.HandleAsync(new CreateInviteRequest(orgId, inviteEmailAddress), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task CreateInvite_RepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            //For the first "control" test we'll set all the variables and assert them down below.
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Create(It.IsAny<Invite>()))
                .ReturnsAsync(new CreateInviteGatewayResponse(null, false,
                    new[] {new Error("500", "Invite Create Failed")}));

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
                await useCase.HandleAsync(new CreateInviteRequest(orgId, inviteEmailAddress), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Invite Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}