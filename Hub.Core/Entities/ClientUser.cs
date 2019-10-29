using System.Collections.Generic;

namespace Hub.Core.Entities
{
    public class ClientUser
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string NickName { get; set; }
        public string Colour { get; set; }
        
        public IList<ClientRank> ClientRanks { get; set; }
    }
}