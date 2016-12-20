namespace PusherServer
{
    public class Notification
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string body { get; set; }

        public string channel { get; set; }
        public string senderId { get; set; }
        public string toId { get; set; }
    }
}
