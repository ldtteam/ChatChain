using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer_WebApp.Data;
using IdentityServer_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class AddClientModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;
        private readonly IConnectionFactory _connectionFactory;

        public AddClientModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupsDbContext groupsContext, IConnectionFactory connectionFactory)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
            _connectionFactory = connectionFactory;
        }

        public Group Group { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Client")]
            public string ClientId { get; set; }
        }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Clients");
            }

            Group = await _groupsContext.Groups.Include(group => group.ClientGroups).ThenInclude(cg => cg.Client).FirstAsync(g => g.Id == id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Clients");
            }
            
            var clients = new List<SelectListItem>();

            foreach (var client in _groupsContext.Clients.Where(c => c.OwnerId == _userManager.GetUserAsync(User).Result.Id))
            {
                if (!Group.ClientGroups.Select(cg => cg.Client).Contains(client))
                {
                    var is4Client = await _is4Context.Clients.FirstOrDefaultAsync(c => c.Id == client.ClientId);

                    if (is4Client != null)
                    {
                        clients.Add(new SelectListItem
                        {
                            Text = is4Client.ClientName,
                            Value = is4Client.Id.ToString()
                        });
                    }
                }
            }

            Clients = clients;
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine($"ClientId: {Input.ClientId}");

            Group = await _groupsContext.Groups.Include(group => group.ClientGroups).ThenInclude(cg => cg.Client).FirstAsync(g => g.Id == id);
            var client = await _groupsContext.Clients.FirstAsync(c => c.ClientId == int.Parse(Input.ClientId));
            var clientGroup = new ClientGroup
            {
                ClientId = client.Id,
                Client = client,
                GroupId = Group.Id,
                Group = Group
            };
            
            Console.WriteLine($"Client: {client}");
            Console.WriteLine($"Thingy: {Group == null}");
            
            Group.ClientGroups.Add(clientGroup);
            await _groupsContext.SaveChangesAsync();
            
            using(var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine("test123456");
                channel.ExchangeDeclare("actions", ExchangeType.Fanout, true, false, null);
                channel.QueueDeclare("actions_queue", true, false, false, null);
                channel.QueueBind("actions_queue", "actions", "");

                var messageEnt = new EventMessage
                {
                    EventName = EventMessage.AddClientEvent,
                    GroupId = Group.GroupId
                };
                    
                string message = JsonConvert.SerializeObject(messageEnt);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                
                var address = new PublicationAddress(ExchangeType.Fanout, "actions", "");
                channel.BasicPublish(address, properties, body);
            }
            
            return RedirectToPage("./Clients");
        }
        
    }
}