namespace ChatChainServer.Models.MessageObjects
{
    public class ClientEventMessage
    {
        public ClientEvent Event { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
    }
}