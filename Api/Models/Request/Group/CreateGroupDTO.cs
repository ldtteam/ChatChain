using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Api.Models.Request.Group
{
    public class CreateGroupDTO
    {
        public string Name { get; set;  }

        public string Description { get; set;  }

        public IList<Guid> ClientIds { get; set;  }
    }
}