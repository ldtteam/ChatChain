using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
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
    public class DeleteGroupUseCase : IDeleteGroupUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IGroupRepository _groupRepository;

        public DeleteGroupUseCase(IOrganisationRepository organisationRepository,
            IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(DeleteGroupRequest message,
            IOutputPort<DeleteGroupResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new DeleteGroupResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.DeleteGroups))
                {
                    outputPort.Handle(new DeleteGroupResponse(new[] {new Error("404", "Group Not Found")},
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
                outputPort.Handle(new DeleteGroupResponse(getConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (getConfigGatewayResponse.Group.OwnerId != organisationGatewayResponse.Organisation.Id)
            {
                //Return an error identical to the "404" so that api groups can't tell the difference between it being non-existent, or not allowed
                outputPort.Handle(new DeleteGroupResponse(new[] {new Error("404", "Group Not Found")},
                    message.UserId != null));
                return false;
            }

            BaseGatewayResponse groupGatewayResponse =
                await _groupRepository.Delete(getConfigGatewayResponse.Group.Id);

            if (!groupGatewayResponse.Success)
            {
                outputPort.Handle(new DeleteGroupResponse(groupGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new DeleteGroupResponse(organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}