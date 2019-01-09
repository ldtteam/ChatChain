namespace IdentityServer_WebApp.Models
{
    public class EventMessage
    {
        public const string DeleteEvent = "group_delete";
        public const string AddClientEvent = "group_add_client";
        public const string RemoveClientEvent = "group_remove_client";
        public string EventName { get; set; }
        public string GroupId { get; set; }
        public string ClientId { get; set; }
    }
}