using System;

namespace ChatChainCommon.DatabaseModels
{
    public class OrganisationInvite
    {
        public Guid OrganisationId { get; set; }
        
        public string Token { get; set; }
        
        public string Email { get; set; }
    }
}