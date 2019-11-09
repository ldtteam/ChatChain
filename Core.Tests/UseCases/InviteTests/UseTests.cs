using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.Invite;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.UseCases.InviteTests
{
    public class UseTests
    {
        [Fact]
        public async Task UseInvite_True()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            bool orgUpdateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => orgUpdateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));

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

            bool inviteDeleteRan = false;
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new GetInviteGatewayResponse(mockInvite, true));
            mockInviteRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback(() => inviteDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // Use Case and OutputPort

            UseInviteUseCase useCase =
                new UseInviteUseCase(mockOrganisationRepository.Object, mockInviteRepository.Object);

            MockOutputPort<UseInviteResponse> mockOutputPort = new MockOutputPort<UseInviteResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UseInviteRequest(orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.NotNull(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(orgUpdateRan);
            Assert.True(inviteDeleteRan);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Null(mockOutputPort.Response.User.Id);
        }

        [Fact]
        public async Task UseInvite_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            bool orgUpdateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => orgUpdateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));

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

            bool inviteDeleteRan = false;
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new GetInviteGatewayResponse(mockInvite, true));
            mockInviteRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback(() => inviteDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // Use Case and OutputPort

            UseInviteUseCase useCase =
                new UseInviteUseCase(mockOrganisationRepository.Object, mockInviteRepository.Object);

            MockOutputPort<UseInviteResponse> mockOutputPort = new MockOutputPort<UseInviteResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UseInviteRequest(orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(orgUpdateRan);
            Assert.False(inviteDeleteRan);
        }

        [Fact]
        public async Task UseInvite_NonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            bool orgUpdateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => orgUpdateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));

            //Invite Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string inviteEmailAddress = "Test@email.com";
            const string inviteToken = "123";

            bool inviteDeleteRan = false;
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new GetInviteGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));
            mockInviteRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback(() => inviteDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // Use Case and OutputPort

            UseInviteUseCase useCase =
                new UseInviteUseCase(mockOrganisationRepository.Object, mockInviteRepository.Object);

            MockOutputPort<UseInviteResponse> mockOutputPort = new MockOutputPort<UseInviteResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UseInviteRequest(orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(orgUpdateRan);
            Assert.False(inviteDeleteRan);
        }

        [Fact]
        public async Task UseInvite_InviteRepDeleteErrors_False()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            bool orgUpdateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => orgUpdateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));

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

            bool inviteDeleteRan = false;
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new GetInviteGatewayResponse(mockInvite, true));
            mockInviteRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback(() => inviteDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(false, new[] {new Error("500", "Invite Delete Failed")}));

            // Use Case and OutputPort

            UseInviteUseCase useCase =
                new UseInviteUseCase(mockOrganisationRepository.Object, mockInviteRepository.Object);

            MockOutputPort<UseInviteResponse> mockOutputPort = new MockOutputPort<UseInviteResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UseInviteRequest(orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Invite Delete Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(orgUpdateRan);
            Assert.True(inviteDeleteRan);
        }

        [Fact]
        public async Task UseInvite_OrgRepUpdateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails {Id = orgId};

            bool orgUpdateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.IsAny<Guid>(), It.IsAny<OrganisationDetails>()))
                .Callback(() => orgUpdateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(null, false,
                    new[] {new Error("500", "Organisation Update Failed")}));

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

            bool inviteDeleteRan = false;
            Mock<IInviteRepository> mockInviteRepository = new Mock<IInviteRepository>();
            mockInviteRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new GetInviteGatewayResponse(mockInvite, true));
            mockInviteRepository
                .Setup(repo => repo.Delete(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback(() => inviteDeleteRan = true)
                .ReturnsAsync(new BaseGatewayResponse(true));

            // Use Case and OutputPort

            UseInviteUseCase useCase =
                new UseInviteUseCase(mockOrganisationRepository.Object, mockInviteRepository.Object);

            MockOutputPort<UseInviteResponse> mockOutputPort = new MockOutputPort<UseInviteResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new UseInviteRequest(orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(orgUpdateRan);
            Assert.True(inviteDeleteRan);
        }
    }
}