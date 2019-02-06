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
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupService _groupsContext;

        public CreateModel(UserManager<ApplicationUser> userManager, GroupService groupsContext)
        {
            _userManager = userManager;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            //[StringLength(25, ErrorMessage = "The {0] must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Group Name")]
            public string GroupName { get; set; }
        }
        
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var groupId = Guid.NewGuid().ToString();

            var group = new Group
            {
                GroupId = groupId,
                GroupName = Input.GroupName,
                OwnerId = _userManager.GetUserAsync(User).Result.Id
            };
            
            _groupsContext.Create(group);
            //await _groupsContext.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
        
    }
}