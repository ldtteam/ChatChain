using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class ApplicationUser : MongoUser
    {
        [PersonalData]
        public string DisplayName { get; set; }
    }
}