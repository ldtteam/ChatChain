using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatChainServer.Data;
using ChatChainServer.Services;
using ChatChainServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Client = IdentityServer4.Models.Client;

namespace ChatChainServer.Hubs
{

    [Authorize]
    public class ChatChainHub:Hub
    {

        private readonly ILogger<ChatChainHub> _logger;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;

        public ChatChainHub(ILogger<ChatChainHub> logger, GroupService groupsContext, ClientService clientsContext)
        {
            _logger = logger;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Connection: {Context.UserIdentifier}");
            foreach (Claim claim in Context.User.Claims)
            {
                _logger.LogInformation($"Claim: {claim.Properties}, {claim.Issuer}, {claim.Value}");
            }
            _logger.LogInformation($"Claims: {Context.User.Claims}");
            
            /*var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (!Startup.ClientIds.ContainsKey(client.ClientGuid))
            {
                Startup.ClientIds.Add(client.ClientGuid, new List<string>());
            }

            Startup.ClientIds[client.ClientGuid].Add(Context.ConnectionId);
            
            //RabbitMq.Invoke(Context, _groupsContext, Groups);
            
            foreach (var cg in client.ClientGroups.FindAll(cg => cg.ClientId == client.Id))
            {
                Groups.AddToGroupAsync(Context.ConnectionId,
                    cg.Group.GroupId);
            }*/

            /*foreach (var cg in client.ClientGroups.FindAll(cg => cg.ClientId == client.Id))
            {
                Clients.Group(cg.Group.GroupId).SendAsync("GenericMessageEvent", "it worked", $"{cg.Group.GroupName}", $"{cg.Group.GroupId}", "", "");
            }*/
            
            return base.OnConnectedAsync();
        }
        
        // ClientType is what ChatChain extension is connecting. E.G. "ChatChainDC", These should be Unique!
        // ClientName is the name of the specific client connecting. E.G. "Minecolonies Test Server", These should be Unique!d
        // Channel is used to specify a chat channel. E.G. "staff" channel.

        public async Task GenericMessageEvent(string clientType, string clientName, string channel, string user, string message)
        {
            _logger.LogInformation($"Client Type: {clientType} of Name: {Context.UserIdentifier} had author: {user} send \"{message}\" in channel: {channel}");
            _logger.LogInformation($"Client: {Context.ConnectionId}, User: {Context.UserIdentifier}");
            //await Clients.All.SendAsync("GenericMessageEvent", clientType, clientName, channel, user, message);
            //await Clients.User("client").SendAsync("GenericMessageEvent", "it worked", "", "", "", "");
            /*var client = await _groupsContext.Clients.Include(c => c.ClientGroups).ThenInclude(cg => cg.Group)
                .FirstAsync(c => c.ClientGuid == Context.UserIdentifier);
            
            foreach (var cg in client.ClientGroups.FindAll(cg => cg.ClientId == client.Id))
            {
                await Clients.Group(cg.Group.GroupId).SendAsync("GenericMessageEvent", $"{clientType}", $"{cg.Group.GroupName}", $"{cg.Group.GroupId}", "", "");
            }*/

            foreach (var group in _clientsContext.GetGroups(_clientsContext.GetFromClientGuid(Context.UserIdentifier).Id
                .ToString()))
            {
                foreach (var client in _groupsContext.GetClients(group.Id.ToString()))
                {
                    await Clients.User(client.ClientGuid).SendAsync("GenericMessageEvent", $"{clientType}",
                        $"{group.GroupName}", $"{group.GroupId}", "", "");
                }
            }
        }

        public async Task GenericJoinEvent(string clientType, string clientName, string channel, string user)
        {
            _logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} join channel: {channel}");
            await Clients.All.SendAsync("GenericJoinEvent", clientType, clientName, channel, user);
        }

        public async Task GenericLeaveEvent(string clientType, string clientName, string channel, string user)
        {
            _logger.LogInformation($"Client Type: {clientType} of Name: {clientName} had user: {user} leave channel: {channel}");
            await Clients.All.SendAsync("GenericLeaveEvent", clientType, clientName, channel, user);
        }
    }
}