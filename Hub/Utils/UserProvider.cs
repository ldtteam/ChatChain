using Microsoft.AspNetCore.SignalR;

namespace Hub.Utils
{
    public class UserProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("client_id")?.Value;
        }
    }
}