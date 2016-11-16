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
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new {info = "user_count"};

            var result = pusherServer.FetchStateForChannel<ChannelStateMessage>(channelName, info);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.User_Count);
        }

        [Test]
        public void It_should_not_return_the_state_based_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel2";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new {info = "does-not-exist"};

            var result = pusherServer.FetchStateForChannel<ChannelStateMessage>(channelName, info);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes",
                (result as GetResult<ChannelStateMessage>).OriginalContent);
        }

        [Test]
        public async void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists()
        {
            var channelName = "presence-state-channel-async-1";
            var reset = new AutoResetEvent(false);

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "user_count" };
            var response = await pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(channelName, info);

            reset.Set();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(1, response.Data.User_Count);
        }

        [Test]
        public async void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists_and_no_info_object_is_provided()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel-async-1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var response = await pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(channelName);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Data.Occupied);
        }

        [Test]
        public async void It_should_not_return_the_state_based_asynchronously_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel-async-2";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "does-not-exist" };

            var result = await pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(channelName, info);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", result.Body);
        }

        [Test]
        public void It_should_throw_an_exception_when_given_an_empty_string_as_a_channel_name()
        {
            var pusherServer = ClientServerFactory.CreateServer();

            var info = new { info = "user_count" };

            ArgumentException caughtException = null;

            try
            {
                pusherServer.FetchStateForChannel<ChannelStateMessage>(string.Empty, info);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
        }

        [Test]
        public void It_should_throw_an_exception_when_given_a_null_as_a_channel_name()
        {
            var pusherServer = ClientServerFactory.CreateServer();

            var info = new { info = "user_count" };

            ArgumentException caughtException = null;

            try
            {
                pusherServer.FetchStateForChannel<ChannelStateMessage>(null, info);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
        }

        [Test]
        public async void It_should_throw_an_exception_when_given_an_empty_string_as_a_channel_name_async()
        {
            var pusherServer = ClientServerFactory.CreateServer();

            var info = new { info = "user_count" };

            ArgumentException caughtException = null;

            try
            {
                var response = await pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(string.Empty, info);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
        }

        [Test]
        public async void It_should_throw_an_exception_when_given_a_null_as_a_channel_name_async()
        {
            var pusherServer = ClientServerFactory.CreateServer();

            var info = new { info = "user_count" };

            ArgumentException caughtException = null;

            try
            {
                var response = await pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(string.Empty, info);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            StringAssert.IsMatch("channelName cannot be null or empty", caughtException.Message);
        }

        private class ChannelStateMessage
        {
            public bool Occupied { get; set; }
            public int User_Count { get; set; }
        }
    }

    [TestFixture]
    public class When_querying_Multiple_Channels
    {
        [Test]
        public void It_should_return_the_state_When_given_a_channel_name_that_exists()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-multiple-state-channel3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new {info = "user_count", filter_by_prefix = "presence-"};

            var result = pusherServer.FetchStateForChannels<object>(info);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, ((Newtonsoft.Json.Linq.JValue)((result.Data as Newtonsoft.Json.Linq.JObject)["channels"]["presence-multiple-state-channel3"]["user_count"])).Value);
        }

        [Test]
        public void It_should_not_return_the_state_based_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-multiple-state-channel4";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new {info = "does-not-exist"};

            var result = pusherServer.FetchStateForChannels<object>(info);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", (result as GetResult<object>).OriginalContent);
        }

        [Test]
        public async void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-multiple-state-channel-async-3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "user_count", filter_by_prefix = "presence-" };

            var result = await pusherServer.FetchStateForChannelsAsync<object>(info);

            reset.Set();

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            
            Assert.AreEqual(1, ((Newtonsoft.Json.Linq.JValue)((result.Data as Newtonsoft.Json.Linq.JObject)["channels"]["presence-multiple-state-channel-async-3"]["user_count"])).Value);
        }

        [Test]
        public async void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists_and_no_info_object_is_provided()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-multiple-state-channel-async-3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var result = await pusherServer.FetchStateForChannelsAsync<object>();

            reset.Set();

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async void It_should_not_return_the_state_asynchronously_based_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-multiple-state-channel-async-4";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "does-not-exist" };

            var result = await pusherServer.FetchStateForChannelsAsync<object>(info);

            reset.Set();

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", result.Body);
        }
    }
}
