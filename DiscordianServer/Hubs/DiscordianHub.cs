using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordianServer.Hubs
{

    public class DiscordianHub:Hub
    {

        private readonly ILogger<DiscordianHub> logger;

        public DiscordianHub(ILogger<DiscordianHub> logger)
        {
            this.logger = logger;
        }

        public async Task GenericAnyDiscordChatMessage(string channel, string author, string message)
        {
            logger.LogInformation($"Discord Chat Message: {author} in channel {channel} - {message}");
            await Clients.All.SendAsync("GenericAnyDiscordChatMessage", channel, author, message);
        }

        // Format for event names:
        // First word is type, Generic, Embed, or etc (or empty).
        // Second is destination.
        // Third is sender.
        // last is descriptor of event.

        // Format for arguments:
        // First (if message is to discord) is the Destination channel.
        // Second is the identifier of the sender (Usually `server`)
        // remander are the arguments of the message.

                    ///  MINECRAFT EVENTS \\\
        public async Task GenericDiscordMinecraftMessage(string channel, string server, string displayMessage)
        {
            logger.LogInformation($"Minecraft Generic Message: {displayMessage}");
            await Clients.All.SendAsync("GenericDiscordMinecraftMessage", channel, server, displayMessage);
        }
        
        public async Task EmbedDiscordMinecraftChatMessage(string channel, string server, string author, string message)
        {
            logger.LogInformation($"Minecraft Chat Message Embed: {author} sent \"{message}\" to {channel}");
            await Clients.All.SendAsync("EmbedDiscordMinecraftChatMessage", channel, server, author, message);
        }

        public async Task AnyMinecraftChatMessage(string server, string author, string message)
        {
            logger.LogInformation($"Minecraft Chat Message: [{server}] {author}: {message}");
            await Clients.All.SendAsync("AnyMinecraftChatMessage", server, author, message);
        }

        public async Task AnyMinecraftPlayerJoin(string server, string username)
        {
            logger.LogInformation($"Minecraft Chat Message: [{server}] {username} joined.");
            await Clients.All.SendAsync("AnyMinecraftPlayerJoin", server, username);
        }

        public async Task AnyMinecraftPlayerLeave(string server, string username)
        {
            logger.LogInformation($"Minecraft Chat Message: [{server}] {username} left.");
            await Clients.All.SendAsync("AnyMinecraftPlayerLeave", server, username);
        }

        public async Task AnyMinecraftServerStart(string server)
        {
            logger.LogInformation($"Minecraft Chat Message: [{server}] started.");
            await Clients.All.SendAsync("AnyMinecraftServerStart", server);
        }

        public async Task AnyMinecraftServerStop(string server)
        {
            logger.LogInformation($"Minecraft Chat Message: [{server}] stopped");
            await Clients.All.SendAsync("AnyMinecraftServerStop", server);
        }
                    /// ^ MINECRAFT EVENTS ^ \\\
    }
}