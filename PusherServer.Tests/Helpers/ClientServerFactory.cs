using System;
using System.Threading;

namespace PusherServer.Tests.Helpers
{
    internal sealed class ClientServerFactory
    {
        /// <summary>
        /// Create a Pusher Client, and subscribes a user
        /// </summary>
        /// <param name="pusherServer">Server to connect to</param>
        /// <param name="reset">The AutoReset to control the subscription by the client</param>
        /// <param name="channelName">The name of the channel to subscribe to</param>
        /// <returns>A subscribed client</returns>
        public static PusherClient.Pusher CreateClient(Pusher pusherServer, AutoResetEvent reset, string channelName)
        {
            PusherClient.Pusher pusherClient =
                new PusherClient.Pusher(Config.AppKey, new PusherClient.PusherOptions()
                {
                    Authorizer = new InMemoryAuthorizer(
                        pusherServer,
                        new PresenceChannelData()
                        {
                            user_id = "Mr Pusher",
                            user_info = new { twitter_id = "@pusher" }
                        })
                })
                {
                    Host = Config.WebSocketHost,
                };

            pusherClient.Connected += delegate { reset.Set(); };

            pusherClient.Connect();

            reset.WaitOne(TimeSpan.FromSeconds(5));

            var channel = pusherClient.Subscribe(channelName);

            channel.Subscribed += delegate { reset.Set(); };

            reset.WaitOne(TimeSpan.FromSeconds(5));

            return pusherClient;
        }

        /// <summary>
        /// Create a Pusher Server instance
        /// </summary>
        /// <returns></returns>
        public static PusherServer.Pusher CreateServer()
        {
            return new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions
            {
                HostName = Config.HttpHost
            });
        }
    }
}
