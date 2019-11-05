using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.OrganisationUser;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.UseCases.OrganisationUserTests
{
    public class GetTests
    {
        [Fact]
        public async Task GetOrganisationUser_True()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();
            //For the first "control" test we'll set all the variables and assert them on the other side.
            IList<OrganisationPermissions> userPermissions = new List<OrganisationPermissions>
                {OrganisationPermissions.All};
            List<OrganisationUser> orgUsers = new List<OrganisationUser>
            {
                new OrganisationUser
                {
                    Id = requestedUserId.ToString(),
                    Permissions = userPermissions
                }
            };

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = orgUsers
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationUserRequest(orgId, requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.RequestedUser.Id, requestedUserId.ToString());

            // Assert Org Values
            Assert.Equal(mockOutputPort.Response.RequestedUser.Permissions, userPermissions);
        }

        [Fact]
        public async Task GetOrganisationUser_NonExistentOrganisation_False()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationUserRequest(orgId, requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.RequestedUser);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetOrganisationUser_NonExistent_False()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();
            //For the first "control" test we'll set all the variables and assert them on the other side.
            string orgOwnerId = Guid.NewGuid().ToString();
            string orgName = Guid.NewGuid().ToString();
            List<OrganisationUser> orgUsers = new List<OrganisationUser>();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = orgOwnerId,
                Name = orgName,
                Users = orgUsers
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationUserRequest(orgId, requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.RequestedUser);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation User Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}