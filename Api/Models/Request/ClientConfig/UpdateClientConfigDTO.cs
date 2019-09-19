using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Api.Models.Request.ClientConfig
{
    public class UpdateClientConfigDTO
    {
        public IEnumerable<Guid> ClientEventGroups { get; set; }
        
        public IEnumerable<Guid> UserEventGroups { get; set; }
    }
}