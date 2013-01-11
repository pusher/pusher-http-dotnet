
namespace Pusher.Server
{
    /// <summary>
    /// Additional options that can be used when triggering an event.
    /// </summary>
    public interface ITriggerOptions
    {
        string SocketId { get; set; }
    }
}
