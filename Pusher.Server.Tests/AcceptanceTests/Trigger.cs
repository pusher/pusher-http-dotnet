using System;
using NUnit.Framework;
using System.Net;

namespace Pusher.Server.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_Triggering_an_Event_on_a_single_Channel
    {
        [Test]
        public void It_should_return_a_202_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger("my-channel", "my_event", new { hello = "world" });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }

    [TestFixture]
    public class When_Triggering_an_Event_on_a_multiple_Channels
    {
        [Test]
        public void It_should_return_a_202_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger(new string[] { "my-channel-1", "my-channel-2" }, "my_event", new { hello = "world" });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
