using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using MongoDB.Bson;

namespace ChatChainCommon.DatabaseServices
{
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetAsync();
        Task<IEnumerable<Group>> GetFromOwnerIdAsync(Guid ownerId);
        Task<Group> GetAsync(Guid groupId);
        Task UpdateAsync(Guid groupId, Group groupIn);
        Task RemoveAsync(Guid groupId);
        Task CreateAsync(Group group);
    }
}