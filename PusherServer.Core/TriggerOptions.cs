
namespace PusherServer
{
    /// <summary>
    /// Represents the Options that can be used by A Trigger
    /// </summary>
    public class TriggerOptions: ITriggerOptions
    {
        /// <summary>
        /// Gets or sets the Socket ID for the consuming Trigger
        /// </summary>
        public string SocketId { get; set; }
    }
}
