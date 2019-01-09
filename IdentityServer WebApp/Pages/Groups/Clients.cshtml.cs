using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer_WebApp.Data;
using IdentityServer_WebApp.Models;
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
        private readonly GroupsDbContext _groupsContext;
        private readonly IConnectionFactory _connectionFactory;
        
        public ClientsModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupsDbContext groupsContext, IConnectionFactory connectionFactory)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
            _connectionFactory = connectionFactory;
        }

        public IList<Client> Clients { get; set; }
        public Group Group { get; set; }
        [BindProperty]
        public int ClientId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Group = await _groupsContext.Groups.Include(group => group.ClientGroups).ThenInclude(cg => cg.Client).FirstAsync(group => group.Id == id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            Clients = new List<Client>();

            List<int> clientIds = new List<int>();
         
            foreach (var client in Group.ClientGroups.Select(cg => cg.Client))
            {
                clientIds.Add(client.ClientId);
            }
            
            foreach (var client in await _is4Context.Clients.ToListAsync())
            {
                var groupClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == client.Id);

                if (groupClient.OwnerId == _userManager.GetUserAsync(User).Result.Id && clientIds.Contains(client.Id))
                {
                    Clients.Add(client);
                }
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int id)
        {
            
            Console.WriteLine($"Group Id: {id}");
            Console.WriteLine($"Client Id: {ClientId}");
            
            Group = await _groupsContext.Groups
                .Include(g => g.ClientGroups)
                .ThenInclude(cg => cg.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            var groupClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == ClientId);
            
            if (Group.OwnerId != _userManager.GetUserAsync(User).Result.Id || groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            var clientId = groupClient.Id;
            
            try
            {
                var clientGroup = Group.ClientGroups.Find(cg => cg.ClientId == clientId);
                Group.ClientGroups.Remove(clientGroup);
                await _groupsContext.SaveChangesAsync();
                
                using(var connection = _connectionFactory.CreateConnection())
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
                }
                
                return RedirectToPage("./Clients");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Clients",
                    new { id = id, saveChangesError = true });
            }
        }
    }
}