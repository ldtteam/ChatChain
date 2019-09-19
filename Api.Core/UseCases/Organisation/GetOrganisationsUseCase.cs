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
    public class GetOrganisationsUseCase : IGetOrganisationsUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public GetOrganisationsUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(GetOrganisationsRequest message,
            IOutputPort<GetOrganisationsResponse> outputPort)
        {
            GetOrganisationsGatewayResponse organisationsGatewayResponse =
                await _organisationRepository.GetForUser(message.UserId);

            if (!organisationsGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetOrganisationsResponse(organisationsGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Dictionary<Guid, Entities.OrganisationUser> organisationUsers =
                new Dictionary<Guid, Entities.OrganisationUser>();

            if (message.UserId != null)
                foreach (OrganisationDetails organisationDetails in organisationsGatewayResponse.Organisations)
                    organisationUsers.Add(organisationDetails.Id,
                        organisationDetails.Users.First(u => u.Id == message.UserId));

            outputPort.Handle(
                new GetOrganisationsResponse(organisationsGatewayResponse.Organisations, organisationUsers,
                    message.UserId != null, true));
            return true;
        }
    }
}