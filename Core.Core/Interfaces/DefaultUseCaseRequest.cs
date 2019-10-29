using System;

namespace Api.Core.Interfaces
{
    public abstract class DefaultUseCaseRequest : IUseCaseRequest
    {
        public string UserId { get; }

        public Guid OrganisationId { get; }

        protected DefaultUseCaseRequest(string userId, Guid organisationId)
        {
            // makes sure that any request made that expects to use a UserID doesn't have a null one. small layer of security.
            UserId = userId ?? throw new InvalidOperationException();
            OrganisationId = organisationId;
        }

        protected DefaultUseCaseRequest(Guid organisationId)
        {
            UserId = null;
            OrganisationId = organisationId;
        }
    }
}