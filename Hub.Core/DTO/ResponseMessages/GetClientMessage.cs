using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.Entities;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.ResponseMessages
{
    public class GetClientMessage : MessageResponse
    {
        public Client Client { get; }

        public GetClientMessage(IEnumerable<Error> errors, bool success = false) : base(errors, success)
        {
        }

        public GetClientMessage(Client client, bool success = false) : base(success)
        {
            Client = client;
        }
    }
}