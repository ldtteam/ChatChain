using System;
using System.Linq;
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
    public class GetsTests
    {
        [Fact]
        public async void GetOrganisationUsers_True()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Guid organisationUserId = Guid.NewGuid();

            OrganisationUser mockOrganisationUser = new OrganisationUser
            {
                Id = organisationUserId.ToString()
            };

            OrganisationDetails mockOrganisation = new OrganisationDetails
                {Id = orgId, Users = new[] {mockOrganisationUser}};

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Use Case and OutputPort

            GetOrganisationUsersUseCase useCase = new GetOrganisationUsersUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationUsersResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUsersResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationUsersRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.User);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Contains(mockOrganisationUser, mockOutputPort.Response.RequestedUsers);
        }

        [Fact]
        public async void GetOrganisationUsers_OrganisationNonExistent_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            // Use Case and OutputPort

            GetOrganisationUsersUseCase useCase = new GetOrganisationUsersUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationUsersResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUsersResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationUsersRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}