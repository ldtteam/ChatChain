using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Group;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Group;
using Api.Core.DTO.UseCaseResponses.Group;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Group;

namespace Api.Core.UseCases.Group
{
    public class UpdateGroupUseCase : IUpdateGroupUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IGroupRepository _groupRepository;

        public UpdateGroupUseCase(IOrganisationRepository organisationRepository,
            IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(UpdateGroupRequest message, IOutputPort<UpdateGroupResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UpdateGroupResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.EditGroups))
                {
                    outputPort.Handle(new UpdateGroupResponse(new[] {new Error("404", "Group Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            GetGroupGatewayResponse getConfigGatewayResponse =
                await _groupRepository.Get(message.GroupId);

            if (!getConfigGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateGroupResponse(getConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (getConfigGatewayResponse.Group.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api groups can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new UpdateGroupResponse(new[] {new Error("404", "Group Not Found")},
                    message.UserId != null));
                return false;
            }

            getConfigGatewayResponse.Group.Name = message.Name;
            getConfigGatewayResponse.Group.Description = message.Description;
            getConfigGatewayResponse.Group.ClientIds = message.ClientIds;

            UpdateGroupGatewayResponse groupGatewayResponse =
                await _groupRepository.Update(message.GroupId, getConfigGatewayResponse.Group);

            if (!groupGatewayResponse.Success)
            {
                outputPort.Handle(new UpdateGroupResponse(groupGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new UpdateGroupResponse(groupGatewayResponse.Group,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}