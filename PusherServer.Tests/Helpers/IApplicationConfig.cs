namespace PusherServer.Tests.Helpers
{
    /// <summary>
    /// The test application configuration settings.
    /// </summary>
    public interface IApplicationConfig
    {
        /// <summary>
        /// Gets or sets the Pusher application id.
        /// </summary>
        string AppId { get; set; }

        /// <summary>
        /// Gets or sets the Pusher application key.
        /// </summary>
        string AppKey { get; set; }

        /// <summary>
        /// Gets or sets the Pusher application secret.
        /// </summary>
        string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the Pusher server API host name. For example, api-mt1.pusher.com.
        /// </summary>
        string HttpHost { get; set; }

        /// <summary>
        /// Gets or sets the Pusher client API host name. For example, ws-mt1.pusher.com.
        /// </summary>
        string WebSocketHost { get; set; }
    }
}
