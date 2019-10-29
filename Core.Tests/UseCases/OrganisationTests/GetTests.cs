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
    public class GetTests
    {
        [Fact]
        public async void GetOrganisation_True()
        {
            // Arrange \\

            Guid orgId = Guid.NewGuid();

            //For the first "control" test we'll set all the variables and assert them on the other side.
            string orgOwnerId = Guid.NewGuid().ToString();
            string orgName = Guid.NewGuid().ToString();
            IEnumerable<OrganisationUser> orgUsers = new List<OrganisationUser>();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = orgOwnerId,
                Name = orgName,
                Users = orgUsers
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            GetOrganisationUseCase useCase = new GetOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationResponse> mockOutputPort = new MockOutputPort<GetOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationRequest(orgId), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);

            // Assert Org Values
            Assert.Equal(mockOutputPort.Response.Organisation.Owner, orgOwnerId);
            Assert.Equal(mockOutputPort.Response.Organisation.Name, orgName);
            Assert.Equal(mockOutputPort.Response.Organisation.Users, orgUsers);
        }

        [Fact]
        public async void GetOrganisation_NonExistent_False()
        {
            // Arrange \\

            Guid orgId = Guid.NewGuid();

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            GetOrganisationUseCase useCase = new GetOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<GetOrganisationResponse> mockOutputPort = new MockOutputPort<GetOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new GetOrganisationRequest(orgId), mockOutputPort);

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