using ChatChainCommon.DatabaseModels;

namespace Api.Models
{
    public class ResponseUser
    {
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Id { get; set; }
        
        public OrganisationUser OrganisationUser { get; set; }
    }
}