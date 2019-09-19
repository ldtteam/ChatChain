using System;
using System.Linq;
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

namespace Api.Tests.Permissions.InviteTests
{
    public class UseTests
    {
        [Fact]
        public async void UseInvite_NonOrgUser_True()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId
            };

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

            bool response = await useCase.HandleAsync(
                new UseInviteRequest(Guid.NewGuid().ToString(), orgId, inviteEmailAddress, inviteToken),
                mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(inviteDeleteRan);
            Assert.True(orgUpdateRan);
        }

        [Fact]
        public async void UseInvite_NotMatchingEmailAddress_False()
        {
            // Arrange \\

            //Organisation Arrange
            Guid orgId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId
            };

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

            bool response =
                await useCase.HandleAsync(new UseInviteRequest(Guid.NewGuid().ToString(), orgId, "", inviteToken),
                    mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(orgUpdateRan);
            Assert.False(inviteDeleteRan);

            // For the first "control" test we'll assert these variables to double check that everything is working.
        }
    }
}