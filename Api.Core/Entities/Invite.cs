using System;

namespace Api.Core.Entities
{
    public class Invite
    {
        public Guid OrganisationId { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }
    }
}