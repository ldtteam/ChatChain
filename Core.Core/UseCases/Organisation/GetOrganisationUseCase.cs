using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Organisation;

namespace Api.Core.UseCases.Organisation
{
    public class GetOrganisationUseCase : IGetOrganisationUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public GetOrganisationUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(GetOrganisationRequest message,
            IOutputPort<GetOrganisationResponse> outputPort)
        {
            DTO.GatewayResponses.Repositories.Organisation.GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetOrganisationResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetOrganisationResponse(new[] {new Error("404", "Organisation Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser = organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            outputPort.Handle(
                new GetOrganisationResponse(organisationGatewayResponse.Organisation, organisationUser,
                    message.UserId != null, true));
            return true;
        }
    }
}