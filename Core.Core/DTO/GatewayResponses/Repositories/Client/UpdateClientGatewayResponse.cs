using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Client
{
    public class UpdateClientGatewayResponse : BaseGatewayResponse
    {
        public Entities.Client Client { get; }

        public UpdateClientGatewayResponse(Entities.Client client, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Client = client;
        }
    }
}