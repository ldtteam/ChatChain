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
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateOrganisation_True()
        {
            // Arrange \\

            // Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Update Values
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

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Update(It.Is<Guid>(id => id.Equals(organisationId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Use Case and OutputPort

            UpdateOrganisationUseCase useCase = new UpdateOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationResponse>
                mockOutputPort = new MockOutputPort<UpdateOrganisationResponse>();


            // Act \\
            bool response = await useCase.HandleAsync(new UpdateOrganisationRequest(organisationId, organisationName),
                mockOutputPort);


            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Empty(mockOutputPort.Response.Errors);
            Assert.Null(mockOutputPort.Response.Message);

            Assert.True(updateRan);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, organisationId);

            // Assert Updated Values.
            Assert.Equal(mockOutputPort.Response.Organisation.Name, organisationName);
            Assert.Equal(mockOutputPort.Response.Organisation.Owner, ownerId.ToString());
            Assert.Equal(mockOutputPort.Response.Organisation.Users, users);
        }

        [Fact]
        public async Task UpdateOrganisation_NonExistent_False()
        {
            // Arrange \\

            Guid organisationId = Guid.NewGuid();

            // Update Values
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

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Update(It.Is<Guid>(id => id.Equals(organisationId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            // Use Case and OutputPort

            UpdateOrganisationUseCase useCase = new UpdateOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationResponse>
                mockOutputPort = new MockOutputPort<UpdateOrganisationResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new UpdateOrganisationRequest(organisationId, ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateOrganisation_RepoUpdateErrors_False()
        {
            // Arrange \\

            //Organisation Arrange

            Guid organisationId = Guid.NewGuid();

            // Update Values
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

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo =>
                    repo.Update(It.Is<Guid>(id => id.Equals(organisationId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(null, false,
                    new[] {new Error("500", "Organisation Update Failed")}));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(organisationId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // Use Case and OutputPort

            UpdateOrganisationUseCase useCase = new UpdateOrganisationUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationResponse>
                mockOutputPort = new MockOutputPort<UpdateOrganisationResponse>();

            // Act \\

            bool response =
                await useCase.HandleAsync(new UpdateOrganisationRequest(organisationId, ""), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.User);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(updateRan);
        }
    }
}