using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupService _groupsContext;
        public readonly ClientService ClientsContext;
        
        public ClientsModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupService groupsContext, ClientService clientsContext)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
            ClientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }
        public Group Group { get; set; }
        [BindProperty]
        public int ClientId { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Group = _groupsContext.Get(id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            Clients = new List<Client>();

            List<int> clientIds = new List<int>();
         
            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                clientIds.Add(client.ClientId);
            }

            foreach (var clientId in clientIds)
            {
                var isClient = await _is4Context.Clients.Where(lclient => lclient.Id == clientId).FirstOrDefaultAsync();
                var client = ClientsContext.GetFromClientId(clientId);
                
                if (isClient != null && client != null && client.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(isClient);
                }    
            }
            
            /*foreach (var client in await _is4Context.Clients.ToListAsync())
            {
                var groupClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == client.Id);

                if (groupClient.OwnerId == _userManager.GetUserAsync(User).Result.Id && clientIds.Contains(client.Id))
                {
                    Clients.Add(client);
                }
            }*/

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            
            Console.WriteLine($"Group Id: {id}");
            Console.WriteLine($"Client Id: {ClientId}");
            
            Group = _groupsContext.Get(id);

            var groupClient = ClientsContext.GetFromClientId(ClientId);
            
            if (Group.OwnerId != _userManager.GetUserAsync(User).Result.Id || groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            var clientId = groupClient.Id;
            
            try
            {
                _groupsContext.RemoveClient(Group.Id, groupClient.Id);
                
                /*using(var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("test12345");
                    channel.ExchangeDeclare("task_queue", "fanout");

                    var messageEnt = new EventMessage
                    {
                        EventName = EventMessage.RemoveClientEvent,
                        GroupId = Group.GroupId,
                        ClientId = clientGroup.Client.ClientGuid
                    };
                    
                    var message = JsonConvert.SerializeObject(messageEnt);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "task_queue",
                        routingKey: "",
                        basicProperties: null,
                        body: body);
                }*/
                
                return RedirectToPage("./Clients", new { id = Group.Id});
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Clients",
                    new { id = Group.Id , saveChangesError = true });
            }
        }
    }
}