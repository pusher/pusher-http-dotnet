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
        IPusher _pusher;

        [TestFixtureSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                Host = Config.Host
            });
        }

        [Test]
        public void It_should_return_a_200_response()
        {
            IGetResult<object> result = _pusher.Get<object>("/channels");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void It_should_be_possible_to_deserialize_the_request_result_body_as_an_object()
        {
            IGetResult<object> result = _pusher.Get<object>("/channels");
            Assert.NotNull(result.Data);
        }

        [Test]
        public void It_should_be_possible_to_deserialize_the_a_channels_result_body_as_an_ChannelsList()
        {
            IGetResult<ChannelsList> result = _pusher.Get<ChannelsList>("/channels");
            Assert.IsTrue(result.Data.Channels.Count >= 0);
        }
    }
}
