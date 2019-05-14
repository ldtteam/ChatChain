namespace ChatChainServer.Models.MessageObjects
{
    public class GenericMessage
    {
        public Group Group { get; set; }
        public ClientUser User { get; set; }
        public string Message { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
    }
}