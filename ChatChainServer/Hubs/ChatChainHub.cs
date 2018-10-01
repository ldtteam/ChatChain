using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatChainServer.Hubs
{

    public class ChatChainHub:Hub
    {

        private readonly ILogger<ChatChainHub> logger;

        public ChatChainHub(ILogger<ChatChainHub> logger)
        {
            this.logger = logger;
        }

        // ClientType is what ChatChain extension is connecting. E.G. "ChatChainDC", These should be Unique!
        // ClientName is the name of the specific cleitn connecting. E.G. "Minecolonies Test Server", These should be Unique!
        // Channel is used to specify a chat channel. E.G. "staff" channel.

        public async Task SendCommand(string clientType, string clientName, string channel, string user, string destinationClient, string command)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} in channel: {channel} send command: {command} to client: {destinationClient}");
            await Clients.All.SendAsync("SendCommand", clientType, clientName, channel, user, destinationClient, command);
        }

        public async Task CommandResponse(string clientType, string clientName, string channel, string user, string destinationClient, string command, string response)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} in channel: {channel} respond to command: {command} from client: {destinationClient} with response: {response}");
            await Clients.All.SendAsync("SendCommand", clientType, clientName, channel, user, destinationClient, command, response);
        }
        public async Task RequestJoined(string clientType, string clientName, string requestedClient)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} requested a joined list from: {requestedClient}");
            await Clients.All.SendAsync("RequestJoined", clientType, clientName, requestedClient);
        }

        public async Task RespondJoined(string clientType, string clientName, string channel, string destinationClient, string message)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} sent joined list response to: {destinationClient} in {channel}");
            await Clients.All.SendAsync("RespondJoined", clientType, clientName, channel, destinationClient, message);
        }

        public async Task GenericConnectionEvent(string clientType, string clientName, string channel)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} connected in channel: {channel}");
            await Clients.All.SendAsync("GenericConnectionEvent", clientType, clientName, channel);
        }

        public async Task GenericDisconnectionEvent(string clientType, string clientName, string channel)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} disconnected in channel: {channel}");
            await Clients.All.SendAsync("GenericDisconnectionEvent", clientType, clientName, channel);
        }

        public async Task GenericMessageEvent(string clientType, string clientName, string channel, string user, string message)
        {
            logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had author: {user} send \"{message}\" in channel: {channel}");
            await Clients.All.SendAsync("GenericMessageEvent", clientType, clientName, channel, user, message);
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