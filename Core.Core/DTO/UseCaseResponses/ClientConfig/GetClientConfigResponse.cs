using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.ClientConfig
{
    public class GetClientConfigResponse : DefaultUseCaseResponseMessage
    {
        public Entities.ClientConfig ClientConfig { get; }

        public GetClientConfigResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public GetClientConfigResponse(Entities.ClientConfig clientConfig, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            ClientConfig = clientConfig;
        }
    }
}