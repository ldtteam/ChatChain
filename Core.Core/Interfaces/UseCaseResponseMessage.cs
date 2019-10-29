using System.Collections.Generic;
using Api.Core.DTO;

namespace Api.Core.Interfaces
{
    public abstract class UseCaseResponseMessage
    {
        public bool Success { get; }

        public string Message { get; }

        public bool CheckedPermissions { get; }

        public IEnumerable<Error> Errors { get; }

        protected UseCaseResponseMessage(IEnumerable<Error> errors, bool checkedPermissions = false,
            bool success = false, string message = null)
        {
            CheckedPermissions = checkedPermissions;
            Errors = errors;
            Success = success;
            Message = message;
        }

        protected UseCaseResponseMessage(bool checkedPermissions = false, bool success = false, string message = null)
        {
            CheckedPermissions = checkedPermissions;
            Success = success;
            Message = message;
            Errors = new List<Error>();
        }
    }
}