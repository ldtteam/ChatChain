using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Group;
using Api.Core.DTO.UseCaseResponses.Group;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Group;

namespace Api.Core.UseCases.Group
{
    public class GetGroupsUseCase : IGetGroupsUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IGroupRepository _groupRepository;

        public GetGroupsUseCase(IOrganisationRepository organisationRepository,
            IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(GetGroupsRequest message,
            IOutputPort<GetGroupsResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetGroupsResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetGroupsResponse(new[] {new Error("404", "Groups Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetGroupsGatewayResponse groupsGatewayResponse =
                await _groupRepository.GetForOwner(organisationGatewayResponse.Organisation.Id);

            if (!groupsGatewayResponse.Success)
            {
                outputPort.Handle(new GetGroupsResponse(groupsGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new GetGroupsResponse(groupsGatewayResponse.Groups,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}