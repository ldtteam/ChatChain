using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Invite
{
    public class CreateInviteGatewayResponse : BaseGatewayResponse
    {
        public Entities.Invite Invite { get; }

        public CreateInviteGatewayResponse(Entities.Invite invite, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Invite = invite;
        }
    }
}