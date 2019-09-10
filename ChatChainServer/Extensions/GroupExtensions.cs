using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;

namespace ChatChainServer.Extensions
{
    public static class GroupExtensions
    {
        public static async Task<List<Client>> GetClientsAsync(this Group group, ClientService clientService)
        {
            List<Client> returnList = new List<Client>();
            foreach (Guid clientId in group.ClientIds)
            {
                returnList.Add(await clientService.GetAsync(clientId));
            }

            return returnList;
        }
    }
}