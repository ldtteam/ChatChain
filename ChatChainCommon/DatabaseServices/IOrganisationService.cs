using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;

namespace ChatChainCommon.DatabaseServices
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Organisation>> GetAsync();
        Task<Organisation> GetAsync(Guid orgId);
        Task<IEnumerable<Organisation>> GetForUserAsync(string userId);
        Task UpdateAsync(Guid orgId, Organisation orgIn);
        Task RemoveAsync(Guid orgId);
        Task CreateAsync(Organisation org);
        Task<OrganisationInvite> GetInviteAsync(Guid orgId, string inviteToken);
        Task RemoveInviteAsync(Guid orgId, string inviteToken);
        Task CreateInviteAsync(OrganisationInvite invite);
    }
}