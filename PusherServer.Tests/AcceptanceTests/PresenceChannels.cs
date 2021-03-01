using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PusherServer.Tests.Helpers;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestClass]
    public class When_querying_the_Presence_Channel
    {
        [TestMethod]
        public async Task Should_get_a_list_of_subscribed_users_asynchronously_when_using_the_correct_channel_name_and_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            IGetResult<PresenceChannelMessage> result = await pusherServer.FetchUsersFromPresenceChannelAsync<PresenceChannelMessage>(channelName);

            reset.Set();

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.Users.Length);
            Assert.AreEqual("Mr Pusher", result.Data.Users[0].Id);
        }

        [TestMethod]
        public async Task Should_get_an_empty_list_of_subscribed_users_asynchronously_when_using_the_correct_channel_name_and_no_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-2";

            var pusherServer = ClientServerFactory.CreateServer();

            IGetResult<PresenceChannelMessage> result = await pusherServer.FetchUsersFromPresenceChannelAsync<PresenceChannelMessage>(channelName);

            reset.Set();

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(0, result.Data.Users.Length);
        }

        [TestMethod]
        public async Task should_return_bad_request_asynchronously_using_an_incorrect_channel_name_and_users_are_subscribed()
        {
            var reset = new AutoResetEvent(false);

            string channelName = "presence-test-channel-async-3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            IGetResult<PresenceChannelMessage> result = await pusherServer.FetchUsersFromPresenceChannelAsync<PresenceChannelMessage>("test-channel-async");

            reset.Set();

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task should_throw_an_exception_when_given_a_null_for_a_channel_name_async()
        {
            var reset = new AutoResetEvent(false);

            var pusherServer = ClientServerFactory.CreateServer();

            ArgumentException caughtException = null;

            try
            {
                var response = await pusherServer.FetchUsersFromPresenceChannelAsync<PresenceChannelMessage>(null);
                reset.Set();
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
        }

        [TestMethod]
        public async Task should_throw_an_exception_when_given_an_empty_string_for_a_channel_name_async()
        {
            var reset = new AutoResetEvent(false);

            var pusherServer = ClientServerFactory.CreateServer();

            ArgumentException caughtException = null;

            try
            {
                var response = await pusherServer.FetchUsersFromPresenceChannelAsync<PresenceChannelMessage>(string.Empty);
                reset.Set();
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
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
