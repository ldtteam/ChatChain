using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class CreateTests
    {
        [Fact]
        public async Task CreateOrganisation_True()
        {
            // Arrange \\

            //Organisation Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            Guid orgId = Guid.NewGuid();
            const string organisationName = "Test Organisation";
            Guid ownerId = Guid.NewGuid();

            List<OrganisationUser> organisationUsers = new List<OrganisationUser>
            {
                new OrganisationUser
                {
                    Id = ownerId.ToString(),
                    Permissions = new List<OrganisationPermissions>
                    {
                        OrganisationPermissions.All
                    }
                }
            };

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Name = organisationName,
                Owner = ownerId.ToString(),
                Users = organisationUsers
            };

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Create(It.IsAny<OrganisationDetails>()))
                .ReturnsAsync(new CreateOrganisationGatewayResponse(mockOrganisation, true));

            // Use Case and OutputPort

            CreateOrganisationUseCase useCase = new CreateOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<CreateOrganisationResponse>
                mockOutputPort = new MockOutputPort<CreateOrganisationResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new CreateOrganisationRequest(ownerId.ToString(), organisationName),
                    mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.NotNull(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);

            // For the first "control" test we'll assert these variables to double check that everything is working.

            // Assert Organisation Values
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.Organisation.Name, organisationName);
            Assert.Equal(mockOutputPort.Response.Organisation.Owner, ownerId.ToString());
            Assert.Equal(mockOutputPort.Response.Organisation.Users, organisationUsers);
        }

        [Fact]
        public async Task CreateOrganisation_RepoCreateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            //For the first "control" test we'll set all the variables and assert them down below.
            const string organisationName = "Test Organisation";

            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Create(It.IsAny<OrganisationDetails>()))
                .ReturnsAsync(new CreateOrganisationGatewayResponse(null, false,
                    new[] {new Error("500", "Organisation Create Failed")}));

            // Use Case and OutputPort

            CreateOrganisationUseCase useCase = new CreateOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<CreateOrganisationResponse>
                mockOutputPort = new MockOutputPort<CreateOrganisationResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(new CreateOrganisationRequest(organisationName), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Create Failed", mockOutputPort.Response.Errors.Select(e => e.Description));
        }
    }
}