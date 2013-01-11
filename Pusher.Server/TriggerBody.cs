namespace Pusher.Server
{
    /// <summary>
    /// Represents the payload to be sent when triggering events
    /// </summary>
    internal class TriggerBody
    {
        public string name { get; set; }

        public string data { get; set; }

        public string[] channels { get; set; }

        public string socket_id { get; set; }
    }
}
