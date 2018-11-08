using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ChatChainServer.Utils
{
    public class ChatChainUserProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("client_id")?.Value;
        }
    }
}