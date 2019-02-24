using System.Collections.Generic;

namespace ChatChainServer.Models.MessageObjects
{
    public class GetGroupsResponseMessage
    {
        public List<Group> Groups { get; set; }
    }
}