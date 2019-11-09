using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Client
{
    public class CreateClientResponse : DefaultUseCaseResponseMessage
    {
        public Entities.Client Client { get; }

        public Entities.ClientConfig ClientConfig { get; }

        public string Password { get; }

        public CreateClientResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) : base(errors, checkedPermissions, success, message)
        {
        }

        public CreateClientResponse(Entities.Client client, Entities.ClientConfig clientConfig, string password,
            Entities.Organisation organisation, Entities.OrganisationUser user, bool checkedPermissions = false,
            bool success = false, string message = null) : base(organisation, user, checkedPermissions, success,
            message)
        {
            Client = client;
            ClientConfig = clientConfig;
            Password = password;
        }
    }
}