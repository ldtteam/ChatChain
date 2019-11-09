using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Organisation
{
    public class DeleteOrganisationRequest : IUseCaseRequest
    {
        public string UserId { get; }

        public Guid Id { get; }

        public DeleteOrganisationRequest(string userId, Guid id)
        {
            UserId = userId ?? throw new InvalidOperationException();
            Id = id;
        }

        public DeleteOrganisationRequest(Guid id)
        {
            Id = id;
        }
    }
}