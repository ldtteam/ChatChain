using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.OrganisationUser;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.Permissions.OrganisationUserTests
{
    public class GetTests
    {
        [Fact]
        public async Task GetOrganisationUser_NonOrgUser_False()
        {
            // Arrange \\
            Guid requestedUserId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = requestedUserId.ToString(),
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = requestedUserId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            Guid userId = Guid.NewGuid();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new GetOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\
            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation User Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));
        }

        [Fact]
        public async Task GetOrganisationUser_OrgUser_True()
        {
            // Arrange \\

            Guid requestedUserId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = Guid.NewGuid().ToString(),
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = requestedUserId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
                    },
                    new OrganisationUser
                    {
                        Id = userId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new GetOrganisationUserRequest(userId.ToString(), Guid.NewGuid(), requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }

        [Fact]
        public async Task GetOrganisationUser_OrgOwner_True()
        {
            // Arrange \\

            Guid requestedUserId = Guid.NewGuid();
            Guid ownerId = Guid.NewGuid();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Owner = ownerId.ToString(),
                Users = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        Id = requestedUserId.ToString(),
                        Permissions = new List<OrganisationPermissions>()
                    },
                    new OrganisationUser
                    {
                        Id = ownerId.ToString()
                    }
                }
            };

            // Mock Organisation Repo that returns mockOrganisation
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            // The Use Case we are testing (UpdateClientConfig)
            GetOrganisationUserUseCase useCase = new GetOrganisationUserUseCase(mockOrganisationRepository.Object);

            //3. The output port is the mechanism to pass response data from the use case to a Presenter
            //for final preparation to deliver to the UI/web page/api response etc.
            MockOutputPort<GetOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<GetOrganisationUserResponse>();

            // Act \\

            //3. We need a request model to cary the data into the use case for the upper layer (UI, Controller, etc.)
            bool response = await useCase.HandleAsync(
                new GetOrganisationUserRequest(ownerId.ToString(), Guid.NewGuid(), requestedUserId.ToString()),
                mockOutputPort);

            // Assert \\
            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.True(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);
        }
    }
}