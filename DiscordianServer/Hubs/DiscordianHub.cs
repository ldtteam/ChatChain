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

        public async Task DiscordChatMessage(string author, string message)
        {
            logger.LogInformation($"Discord: {author} - {message}");
            await Clients.All.SendAsync("DiscordChatMessage", author, message);
        }

        public async Task MinecraftChatMessage(string author, string message)
        {
            logger.LogInformation($"Minecraft: {author} - {message}");
            await Clients.All.SendAsync("MinecraftChatMessage", author, message);
        }
    }
}