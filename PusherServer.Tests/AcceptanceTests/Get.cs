using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using NUnit.Framework;
using System.IO;
using System.Web.Script.Serialization;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_application_channels_are_queried
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
        }

        [Test]
        public void It_should_return_a_200_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<object> result = pusher.Get<object>("/channels");

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void It_should_be_possible_to_deserialize_the_request_result_body_as_an_object()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<object> result = pusher.Get<object>("/channels");

            Assert.NotNull(result.Data);
        }

        [Test]
        public void It_should_be_possible_to_deserialize_the_a_channels_result_body_as_an_ChannelsList()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<ChannelsList> result = pusher.Get<ChannelsList>("/channels");

            Assert.IsTrue(result.Data.Channels.Count >= 0);
        }
    }
}
