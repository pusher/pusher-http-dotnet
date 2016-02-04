using System;
using System.Collections.Generic;
using System.Collections.Generic;
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
        public void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel-async-1";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "user_count" };

            IGetResult<ChannelStateMessage> result = null;

            pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(channelName, info, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Data.User_Count);
        }

        [Test]
        public void It_should_not_return_the_state_based_asynchronously_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-state-channel-async-2";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "does-not-exist" };

            IGetResult<ChannelStateMessage> result = null;

            pusherServer.FetchStateForChannelAsync<ChannelStateMessage>(channelName, info, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", (result as GetResult<ChannelStateMessage>).OriginalContent);
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
            // Really need to introduce a mechanism to use a different deserialiser!
            Assert.AreEqual(1, ((((Dictionary<string, object>)result.Data)["channels"] as Dictionary<string, object>)["presence-multiple-state-channel3"] as Dictionary<string, object>)["user_count"]);
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
        public void It_should_return_the_state_asynchronously_When_given_a_channel_name_that_exists()
        {
            var reset = new AutoResetEvent(false);

            var channelName = "presence-multiple-state-channel-async-3";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "user_count", filter_by_prefix = "presence-" };

            IGetResult<object> result = null;

            pusherServer.FetchStateForChannelsAsync<object>(info, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            // Really need to introduce a mechanism to use a different deserialiser!
            Assert.AreEqual(1, ((((Dictionary<string, object>)result.Data)["channels"] as Dictionary<string, object>)["presence-multiple-state-channel-async-3"] as Dictionary<string, object>)["user_count"]);
        }

        [Test]
        public void It_should_not_return_the_state_asynchronously_based_When_given_a_channel_name_that_exists_an_bad_attributes()
        {
            AutoResetEvent reset = new AutoResetEvent(false);

            string channelName = "presence-multiple-state-channel-async-4";

            var pusherServer = ClientServerFactory.CreateServer();
            var pusherClient = ClientServerFactory.CreateClient(pusherServer, reset, channelName);

            var info = new { info = "does-not-exist" };

            IGetResult<object> result = null;

            pusherServer.FetchStateForChannelsAsync<object>(info, getResult =>
            {
                result = getResult;
                reset.Set();
            });

            reset.WaitOne(TimeSpan.FromSeconds(30));

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            StringAssert.IsMatch("info should be a comma separated list of attributes", (result as GetResult<object>).OriginalContent);
        }
    }
}
