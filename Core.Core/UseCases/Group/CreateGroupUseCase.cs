using System;
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
    public class CreateGroupUseCase : ICreateGroupUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IGroupRepository _groupRepository;

        public CreateGroupUseCase(IOrganisationRepository organisationRepository,
            IGroupRepository groupRepository)
        {
            _organisationRepository = organisationRepository;
            _groupRepository = groupRepository;
        }

        public async Task<bool> HandleAsync(CreateGroupRequest message, IOutputPort<CreateGroupResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new CreateGroupResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.CreateGroups))
                {
                    outputPort.Handle(new CreateGroupResponse(new[] {new Error("404", "Organisation Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            Entities.Group group = new Entities.Group
            {
                Id = Guid.NewGuid(),
                OwnerId = organisationGatewayResponse.Organisation.Id,
                Name = message.Name,
                Description = message.Description,
                ClientIds = message.ClientIds
            };

            CreateGroupGatewayResponse groupGatewayResponse = await _groupRepository.Create(group);

            if (!groupGatewayResponse.Success)
            {
                outputPort.Handle(new CreateGroupResponse(groupGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new CreateGroupResponse(groupGatewayResponse.Group,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}