using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;

namespace IdentityServer_WebApp.Models
{
    public class ApplicationUser : MongoUser
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }
    }
}