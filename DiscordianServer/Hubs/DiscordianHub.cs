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

        public async Task DiscordChatMessage(string author, string channel, string message)
        {
            logger.LogInformation($"Discord: {author} in channel {channel} - {message}");
            await Clients.All.SendAsync("DiscordChatMessage", author, channel, message);
        }

        public async Task MinecraftChatMessage(string author, string server, string message)
        {
            logger.LogInformation($"Minecraft: {author} in server {server} - {message}");
            await Clients.All.SendAsync("MinecraftChatMessage", author, server, message);
        }
    }
}