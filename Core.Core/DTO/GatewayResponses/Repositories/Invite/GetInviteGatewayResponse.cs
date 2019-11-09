using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Invite
{
    public class GetInviteGatewayResponse : BaseGatewayResponse
    {
        public Entities.Invite Invite { get; }

        public GetInviteGatewayResponse(Entities.Invite invite, bool success = false, IEnumerable<Error> errors = null)
            : base(success, errors)
        {
            Invite = invite;
        }
    }
}