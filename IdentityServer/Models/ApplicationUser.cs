using AspNetCore.Identity.Mongo.Model;

namespace IdentityServer.Models
{
    public class ApplicationUser : MongoUser
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }
    }
}