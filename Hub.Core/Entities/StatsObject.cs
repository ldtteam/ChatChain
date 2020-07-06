using System.Collections.Generic;

namespace Hub.Core.Entities
{
    public class StatsObject
    {
        public List<ClientUser> OnlineUsers { get; set; }
        
        public string Performance { get; set; }
        
        public string PerformanceName { get; set; }
    }
}