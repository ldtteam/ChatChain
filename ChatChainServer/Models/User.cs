using System.Collections.Generic;

namespace ChatChainServer.Models
{
    public class User
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        
        public List<ClientRank> ClientRanks { get; set; }
        
    }
}