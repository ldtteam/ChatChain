using System.Collections.Generic;
using Api.Core.DTO;

namespace Hub.Core.Interfaces
{
    public abstract class MessageResponse
    {
        public bool Success { get; }
        
        public IEnumerable<Error> Errors { get; }

        protected MessageResponse(IEnumerable<Error> errors, bool success = false)
        {
            Errors = errors;
            Success = success;
        }
        
        protected MessageResponse(bool success = false)
        {
            Success = success;
            Errors = new List<Error>();
        }
    }
}