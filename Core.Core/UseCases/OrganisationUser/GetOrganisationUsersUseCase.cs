using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.OrganisationUser;

namespace Api.Core.UseCases.OrganisationUser
{
    public class GetOrganisationUsersUseCase : IGetOrganisationUsersUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public GetOrganisationUsersUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(GetOrganisationUsersRequest message,
            IOutputPort<GetOrganisationUsersResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetOrganisationUsersResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetOrganisationUsersResponse(
                        new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                    return false;
                }

                organisationUser = organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            IEnumerable<Entities.OrganisationUser> organisationUsers = organisationGatewayResponse.Organisation.Users;

            outputPort.Handle(
                new GetOrganisationUsersResponse(organisationUsers, organisationGatewayResponse.Organisation.ToOrganisation(),
                    organisationUser, message.UserId != null, true));
            return true;
        }
    }
}