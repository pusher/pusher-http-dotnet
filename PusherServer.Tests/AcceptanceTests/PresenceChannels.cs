using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using PusherServer.Tests.Helpers;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_querying_the_Presence_Channel
    {
        [Test]
        public void Should_get_a_list_of_subscribed_users_when_using_the_correct_channel_name_and_users_are_subscribed()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-channel1";

            var pusherServer = CreateServer();
            var pusherClient = CreateClient(pusherServer, reset, channelName);

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>(channelName);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.Users.Length);
            Assert.AreEqual("Mr Pusher", result.Data.Users[0].Id);
        }

        [Test]
        public void Should_get_an_empty_list_of_subscribed_users_when_using_the_correct_channel_name_and_no_users_are_subscribed()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-channel2";

            var pusherServer = CreateServer();

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>(channelName);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(0, result.Data.Users.Length);
        }

        [Test]
        public void should_return_bad_request_using_an_incorrect_channel_name_and_users_are_subscribed()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-channel3";

            var pusherServer = CreateServer();
            var pusherClient = CreateClient(pusherServer, reset, channelName);

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>("test-channel");

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
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
                            user_info = new {twitter_id = "@pusher"}
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

        private class PresenceChannelMessage
        {
            public PresenceChannelUser[] Users { get; set; }
        }

        private class PresenceChannelUser
        {
            public string Id { get; set; }
        }
    }
}
