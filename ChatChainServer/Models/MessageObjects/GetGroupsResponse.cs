using System.Collections.Generic;
using ChatChainCommon.DatabaseModels;

namespace ChatChainServer.Models.MessageObjects
{
    public class GetGroupsResponse
    {
        public List<Group> Groups { get; set; }
    }
}