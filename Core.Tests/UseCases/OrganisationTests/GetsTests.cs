using System;
using System.Collections.Generic;
using System.Linq;
using Api.Core.DTO;
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
    public class GetsTests
    {
        [Fact]
        public async void GetOrganisations_True()
        {
            // Arrange \\

            Guid orgId = Guid.NewGuid();

            Guid userId = Guid.NewGuid();

            //For the first "control" test we'll set all the variables and assert them on the other side.
            string orgOwnerId = Guid.NewGuid().ToString();
            string orgName = Guid.NewGuid().ToString();
            IEnumerable<OrganisationUser> orgUsers = new List<OrganisationUser>
                {new OrganisationUser {Id = userId.ToString()}};

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = orgOwnerId,
                Name = orgName,
                Users = orgUsers
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.GetForUser(It.IsAny<string>()))
                .ReturnsAsync(new GetOrganisationsGatewayResponse(new[] {mockOrganisation}, true));

            GetOrganisationsUseCase useCase = new GetOrganisationsUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationsResponse> mockOutputPort = new MockOutputPort<GetOrganisationsResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationsRequest(userId.ToString()), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.NotEmpty(mockOutputPort.Response.Users);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.Contains(mockOrganisation, mockOutputPort.Response.Organisations);
            Assert.Contains(userId.ToString(), mockOutputPort.Response.Users.Values.Select(u => u.Id));
        }

        [Fact]
        public async void GetOrganisations_RepoGetsErrors_False()
        {
            // Arrange \\

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.GetForUser(It.IsAny<string>()))
                .ReturnsAsync(new GetOrganisationsGatewayResponse(null, false,
                    new[] {new Error("500", "Organisation GetForUser Failed")}));

            GetOrganisationsUseCase useCase = new GetOrganisationsUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationsResponse> mockOutputPort = new MockOutputPort<GetOrganisationsResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationsRequest(), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation GetForUser Failed",
                mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}