using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Invite;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.Services;
using Api.Core.Interfaces.UseCases.Invite;

namespace Api.Core.UseCases.Invite
{
    public class CreateInviteUseCase : ICreateInviteUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IInviteRepository _inviteRepository;
        private readonly IPasswordGenerator _passwordGenerator;

        public CreateInviteUseCase(IOrganisationRepository organisationRepository, IInviteRepository inviteRepository,
            IPasswordGenerator passwordGenerator)
        {
            _organisationRepository = organisationRepository;
            _inviteRepository = inviteRepository;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<bool> HandleAsync(CreateInviteRequest message, IOutputPort<CreateInviteResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new CreateInviteResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.CreateOrgUsers))
                {
                    outputPort.Handle(new CreateInviteResponse(new[] {new Error("404", "Organisation Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }

            Entities.Invite invite = new Entities.Invite
            {
                OrganisationId = organisationGatewayResponse.Organisation.Id,
                Email = message.EmailAddress,
                Token = _passwordGenerator.GenerateNoSpecial()
            };

            CreateInviteGatewayResponse inviteGatewayResponse = await _inviteRepository.Create(invite);

            if (!inviteGatewayResponse.Success)
            {
                outputPort.Handle(new CreateInviteResponse(inviteGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new CreateInviteResponse(inviteGatewayResponse.Invite,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}