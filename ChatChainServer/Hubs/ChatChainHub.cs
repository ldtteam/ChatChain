using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatChainServer.Hubs
{

    [Authorize]
    public class ChatChainHub:Hub
    {

        private readonly ILogger<ChatChainHub> logger;

        public ChatChainHub(ILogger<ChatChainHub> logger)
        {
            this.logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            logger.LogInformation($"Connection: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }
        /*public override Task OnDisconnectedAsync(Exception exception)
        {   
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} disconnected in channel: {channel}");
            await Clients.All.SendAsync("GenericDisconnectionEvent", clientType, clientName, channel);
            return base.OnDisconnectedAsync(exception);
        }*/
        
        // ClientType is what ChatChain extension is connecting. E.G. "ChatChainDC", These should be Unique!
        // ClientName is the name of the specific client connecting. E.G. "Minecolonies Test Server", These should be Unique!d
        // Channel is used to specify a chat channel. E.G. "staff" channel.

        public async Task GenericMessageEvent(string clientType, string clientName, string channel, string user, string message)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {Context.UserIdentifier} had author: {user} send \"{message}\" in channel: {channel}");
            logger.LogInformation($"Client: {Context.ConnectionId}, User: {Context.UserIdentifier}");
            await Clients.All.SendAsync("GenericMessageEvent", clientType, clientName, channel, user, message);
            await Clients.User("client").SendAsync("GenericMessageEvent", "it worked", "", "", "", "");
        }

        public async Task GenericJoinEvent(string clientType, string clientName, string channel, string user)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} join channel: {channel}");
            await Clients.All.SendAsync("GenericJoinEvent", clientType, clientName, channel, user);
        }

        public async Task GenericLeaveEvent(string clientType, string clientName, string channel, string user)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} leave channel: {channel}");
            await Clients.All.SendAsync("GenericLeaveEvent", clientType, clientName, channel, user);
        }
    }
}