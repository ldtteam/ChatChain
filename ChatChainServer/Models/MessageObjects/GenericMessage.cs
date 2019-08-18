namespace ChatChainServer.Models.MessageObjects
{
    public class GenericMessage
    {
        public Group Group { get; set; }
        public ClientUser ClientUser { get; set; }
        public string Message { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
    }
}