using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Organisation
{
    public class CreateOrganisationRequest : UseCaseRequest
    {
        public string UserId { get; }

        public string Name { get; }

        public CreateOrganisationRequest(string userId, string name)
        {
            UserId = userId ?? throw new InvalidOperationException();
            Name = name;
        }

        public CreateOrganisationRequest(string name)
        {
            Name = name;
        }
    }
}