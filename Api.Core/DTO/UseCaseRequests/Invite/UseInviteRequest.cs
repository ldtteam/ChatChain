using System;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseRequests.Invite
{
    public class UseInviteRequest : DefaultUseCaseRequest
    {
        public string UserEmailAddress { get; }

        public string Token { get; }

        public UseInviteRequest(string userId, Guid organisationId, string userEmailAddress, string token) : base(
            userId, organisationId)
        {
            UserEmailAddress = userEmailAddress;
            Token = token;
        }

        public UseInviteRequest(Guid organisationId, string userEmailAddress, string token) : base(organisationId)
        {
            UserEmailAddress = userEmailAddress;
            Token = token;
        }
    }
}