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
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>(channelName);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.Users.Length);
            Assert.AreEqual("Mr Pusher", result.Data.Users[0].Id);
        }

        [Test]
        public void Should_get_an_empty_list_of_subscribed_users_when_using_the_correct_channel_name_and_no_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel2";

            var pusherServer = ClientServerFactory.CreateServer();

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>(channelName);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(0, result.Data.Users.Length);
        }

        [Test]
        public void should_return_bad_request_using_an_incorrect_channel_name_and_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var result = pusherServer.FetchUsersFromPrecenceChannel<PresenceChannelMessage>("test-channel");

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void Should_get_a_list_of_subscribed_users_asynchronously_when_using_the_correct_channel_name_and_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            IGetResult<PresenceChannelMessage> result = null;
            pusherServer.FetchUsersFromPrecenceChannelAsync<PresenceChannelMessage>(channelName, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.Users.Length);
            Assert.AreEqual("Mr Pusher", result.Data.Users[0].Id);
        }

        [Test]
        public void Should_get_an_empty_list_of_subscribed_users_asynchronously_when_using_the_correct_channel_name_and_no_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-2";

            var pusherServer = ClientServerFactory.CreateServer();

            IGetResult<PresenceChannelMessage> result = null;
            pusherServer.FetchUsersFromPrecenceChannelAsync<PresenceChannelMessage>(channelName, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(0, result.Data.Users.Length);
        }

        [Test]
        public void should_return_bad_request_asynchronously_using_an_incorrect_channel_name_and_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            IGetResult<PresenceChannelMessage> result = null;
            pusherServer.FetchUsersFromPrecenceChannelAsync<PresenceChannelMessage>("test-channel-async", getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
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
