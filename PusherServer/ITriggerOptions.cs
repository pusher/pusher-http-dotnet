namespace PusherServer
{
    /// <summary>
    /// Additional options that can be used when triggering an event.
    /// </summary>
    public interface ITriggerOptions
    {
        /// <summary>
        /// Gets or sets the Socket ID for a consuming Trigger
        /// </summary>
        string SocketId { get; set; }
    }
}
