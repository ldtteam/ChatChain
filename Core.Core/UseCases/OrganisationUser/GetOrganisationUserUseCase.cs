using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.OrganisationUser;

namespace Api.Core.UseCases.OrganisationUser
{
    public class GetOrganisationUserUseCase : IGetOrganisationUserUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;

        public GetOrganisationUserUseCase(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> HandleAsync(GetOrganisationUserRequest message,
            IOutputPort<GetOrganisationUserResponse> outputPort)
        {
            DTO.GatewayResponses.Repositories.Organisation.GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetOrganisationUserResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetOrganisationUserResponse(
                        new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                    return false;
                }

                organisationUser = organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            if (!organisationGatewayResponse.Organisation.UserIsMember(message.OrganisationUserId))
            {
                outputPort.Handle(new GetOrganisationUserResponse(
                    new[] {new Error("404", "Organisation User Not Found")}, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser requestedOrganisationUser =
                organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.OrganisationUserId);

            outputPort.Handle(
                new GetOrganisationUserResponse(requestedOrganisationUser, organisationGatewayResponse.Organisation.ToOrganisation(),
                    organisationUser, message.UserId != null, true));
            return true;
        }
    }
}