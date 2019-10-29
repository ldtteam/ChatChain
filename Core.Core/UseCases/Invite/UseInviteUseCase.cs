using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.UseCases.Invite;

namespace Api.Core.UseCases.Invite
{
    public class UseInviteUseCase : IUseInviteUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IInviteRepository _inviteRepository;

        public UseInviteUseCase(IOrganisationRepository organisationRepository, IInviteRepository inviteRepository)
        {
            _organisationRepository = organisationRepository;
            _inviteRepository = inviteRepository;
        }

        public async Task<bool> HandleAsync(UseInviteRequest message, IOutputPort<UseInviteResponse> outputPort)
        {
            GetOrganisationGatewayResponse getOrganisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!getOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UseInviteResponse(getOrganisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            GetInviteGatewayResponse getInviteGatewayResponse =
                await _inviteRepository.Get(getOrganisationGatewayResponse.Organisation.Id, message.Token);

            if (!getInviteGatewayResponse.Success)
            {
                outputPort.Handle(
                    new UseInviteResponse(getInviteGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            if (!getInviteGatewayResponse.Invite.Email.Equals(message.UserEmailAddress))
            {
                outputPort.Handle(
                    new UseInviteResponse(new[] {new Error("404", "Organisation Not Found")}, message.UserId != null));
                return false;
            }

            // As a failsafe we always attempt to delete an invite before adding the user to the organisation.

            BaseGatewayResponse deleteInviteGatewayResponse =
                await _inviteRepository.Delete(getOrganisationGatewayResponse.Organisation.Id, message.Token);

            if (!deleteInviteGatewayResponse.Success)
            {
                outputPort.Handle(new UseInviteResponse(deleteInviteGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = new Entities.OrganisationUser
            {
                Id = message.UserId
            };

            List<Entities.OrganisationUser> users = getOrganisationGatewayResponse.Organisation.Users.ToList();
            users.Add(organisationUser);
            getOrganisationGatewayResponse.Organisation.Users = users;

            UpdateOrganisationGatewayResponse updateOrganisationGatewayResponse =
                await _organisationRepository.Update(getOrganisationGatewayResponse.Organisation.Id,
                    getOrganisationGatewayResponse.Organisation);

            if (!updateOrganisationGatewayResponse.Success)
            {
                outputPort.Handle(new UseInviteResponse(updateOrganisationGatewayResponse.Errors,
                    message.UserId != null));
                return false;
            }

            outputPort.Handle(new UseInviteResponse(getOrganisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}