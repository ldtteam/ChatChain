using System.Collections.Generic;
using ChatChainCommon.DatabaseModels;

namespace ChatChainServer.Models.MessageObjects
{
    public class GetGroupsResponse
    {
        public IEnumerable<Group> Groups { get; set; }
    }
}