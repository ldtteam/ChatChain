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
    public class GetGroupUseCase : IGetGroupUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IGroupRepository _groupRepository;

        public GetGroupUseCase(IOrganisationRepository organisationRepository,
            IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(GetGroupRequest message,
            IOutputPort<GetGroupResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new GetGroupResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserIsMember(message.UserId))
                {
                    outputPort.Handle(new GetGroupResponse(new[] {new Error("404", "Group Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetGroupGatewayResponse groupGatewayResponse =
                await _groupRepository.Get(message.GroupId);

            if (!groupGatewayResponse.Success)
            {
                outputPort.Handle(new GetGroupResponse(groupGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (groupGatewayResponse.Group.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api Group's can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new GetGroupResponse(new[] {new Error("404", "Group Not Found")},
                    message.UserId != null));
                return false;
            }

            outputPort.Handle(new GetGroupResponse(groupGatewayResponse.Group,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}