using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Client
{
    public class CreateClientGatewayResponse : BaseGatewayResponse
    {
        public Entities.Client Client { get; }

        public CreateClientGatewayResponse(Entities.Client client, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Client = client;
        }
    }
}