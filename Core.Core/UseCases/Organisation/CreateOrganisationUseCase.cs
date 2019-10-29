using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Organisation;

namespace Api.Core.UseCases.Organisation
{
    public class CreateOrganisationUseCase : ICreateOrganisationUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public CreateOrganisationUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(CreateOrganisationRequest message,
            IOutputPort<CreateOrganisationResponse> outputPort)
        {
            OrganisationDetails organisation = new OrganisationDetails
            {
                Id = Guid.NewGuid(),
                Name = message.Name,
                Owner = message.UserId,
                Users = new List<Entities.OrganisationUser>
                {
                    new Entities.OrganisationUser
                    {
                        Id = message.UserId
                    }
                }
            };

            CreateOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Create(organisation);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new CreateOrganisationResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);

            outputPort.Handle(new CreateOrganisationResponse(organisationGatewayResponse.Organisation, organisationUser,
                message.UserId != null, true));
            return true;
        }
    }
}