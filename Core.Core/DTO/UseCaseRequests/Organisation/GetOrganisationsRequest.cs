using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Organisation
{
    public class GetOrganisationsRequest : IUseCaseRequest
    {
        public string UserId { get; }

        public GetOrganisationsRequest(string userId)
        {
            UserId = userId ?? throw new InvalidOperationException();
        }

        public GetOrganisationsRequest()
        {
        }
    }
}