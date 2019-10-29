using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Invite
{
    public class CreateInviteRequest : DefaultUseCaseRequest
    {
        public string EmailAddress { get; }

        public CreateInviteRequest(string userId, Guid organisationId, string emailAddress) : base(userId,
            organisationId)
        {
            EmailAddress = emailAddress;
        }

        public CreateInviteRequest(Guid organisationId, string emailAddress) : base(organisationId)
        {
            EmailAddress = emailAddress;
        }
    }
}