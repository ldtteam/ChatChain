using System.Collections.Generic;
using ChatChainServer.Models.CommandObjects;

namespace ChatChainServer.Models.MessageObjects
{
    public class GetCommandsResponse
    {
        public List<Command> Commands { get; set; }
    }
}