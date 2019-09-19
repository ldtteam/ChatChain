using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses
{
    public class BaseGatewayResponse
    {
        public bool Success { get; }
        public IEnumerable<Error> Errors { get; }

        public BaseGatewayResponse(bool success = false, IEnumerable<Error> errors = null)
        {
            Success = success;
            Errors = errors;
            if (errors == null)
                Errors = new List<Error>();
        }
    }
}