using System.Collections.Generic;

namespace ChatChainServer.Models.MessageObjects
{
    public class GetGroupsResponse
    {
        public List<Group> Groups { get; set; }
    }
}