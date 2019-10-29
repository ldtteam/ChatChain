using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages
{
    public class GetGroupsMessage : MessageResponse
    {
        public IList<Group> Groups { get; }

        public GetGroupsMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public GetGroupsMessage(IList<Group> groups, bool success = false) : base(success)
        {
            Groups = groups;
        }
    }
}