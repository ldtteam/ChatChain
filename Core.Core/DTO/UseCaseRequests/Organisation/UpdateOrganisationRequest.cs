using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Organisation
{
    public class UpdateOrganisationRequest : DefaultUseCaseRequest
    {
        public string Name { get; }

        public UpdateOrganisationRequest(string userId, Guid organisationId, string name) : base(userId, organisationId)
        {
            Name = name;
        }

        public UpdateOrganisationRequest(Guid organisationId, string name) : base(organisationId)
        {
            Name = name;
        }
    }
}