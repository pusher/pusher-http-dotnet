using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using PusherServer.Tests.Helpers;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_querying_a_Channel
    {
        [Test]
        public void It_should_return_the_state_When_given_a_channel_name_that_exists()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-channel1";

            var pusherServer = CreateServer();
            var pusherClient = CreateClient(pusherServer, reset, channelName);

            var info = new {info = "user_count" };

            var result = pusherServer.FetchStateForChannel<ChannelStateMessage>(channelName, info);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.User_Count);
        }

        [Test]
        public void It_should_not_return_the_state_based_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-channel1";

            var pusherServer = CreateServer();
            var pusherClient = CreateClient(pusherServer, reset, channelName);

            var info = new { info = "does-not-exist" };

            var result = pusherServer.FetchStateForChannel<ChannelStateMessage>(channelName, info);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", (result as GetResult<ChannelStateMessage>).OriginalContent);
        }

        /// <summary>
        /// Create a Pusher Client, and subscribes a user
        /// </summary>
        /// <param name="pusherServer">Server to connect to</param>
        /// <param name="reset">The AutoReset to control the subscription by the client</param>
        /// <param name="channelName">The name of the channel to subscribe to</param>
        /// <returns>A subscribed client</returns>
        private static PusherClient.Pusher CreateClient(Pusher pusherServer, AutoResetEvent reset, string channelName)
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
                });

            pusherClient.Connected += delegate { reset.Set(); };

            pusherClient.Connect();

            reset.WaitOne(TimeSpan.FromSeconds(5));

            var channel = pusherClient.Subscribe(channelName);

            channel.Subscribed += delegate { reset.Set(); };

            reset.WaitOne(TimeSpan.FromSeconds(5));

            return pusherClient;
        }

        private PusherServer.Pusher CreateServer()
        {
            return new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
        }

        private class ChannelStateMessage
        {
            public bool Occupied { get; set; }
            public int User_Count { get; set; }
            public int Subscription_Count { get; set; }
        }
    }
}
