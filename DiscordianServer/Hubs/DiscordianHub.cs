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

        public async Task DiscordChatMessage(string channel, string author, string message)
        {
            logger.LogInformation($"Discord Chat Message: {author} in channel {channel} - {message}");
            await Clients.All.SendAsync("DiscordChatMessage", channel, author, message);
        }

        public async Task MinecraftGenericMessage(string channel, string server, string displayMessage)
        {
            logger.LogInformation($"Minecraft Generic Message: {displayMessage}");
            await Clients.All.SendAsync("MinecraftGenericMessage", channel, server, displayMessage);
        }
        
        public async Task MinecraftChatMessageEmbed(string channel, string server, string author, string message)
        {
            logger.LogInformation($"Minecraft Chat Message Embed: {author} sent \"{message}\" to {channel}");
            await Clients.All.SendAsync("MinecraftChatMessageEmbed", channel, server, author, message);
        }
    }
}