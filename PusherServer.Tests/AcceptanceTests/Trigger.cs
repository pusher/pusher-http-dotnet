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
    public class When_Triggering_an_Event_on_a_single_Channel
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));

        }

        [Test]
        public void It_should_return_a_200_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger("my-channel", "my_event", new { hello = "world" });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void it_should_expose_the_event_id()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger("my-channel", "my_event", new { hello = "world" });
            Assert.IsTrue(string.IsNullOrEmpty(result.EventIds["my-channel"]) == false);
        }

        [Test]
        public void It_should_be_received_by_a_client()
        {
            string channelName = "my_channel";
            string eventName = "my_event";

            bool eventReceived = false;
            AutoResetEvent reset = new AutoResetEvent(false);
            var client = new PusherClient.Pusher(Config.AppKey);
            client.Connected += new PusherClient.ConnectedEventHandler(delegate(object sender)
            {
                Debug.WriteLine("connected");
                reset.Set();
            });

            Debug.WriteLine("connecting");
            client.Connect();

            Debug.WriteLine("waiting to connect");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Debug.WriteLine("subscribing");
            var channel = client.Subscribe(channelName);
            channel.Subscribed += new PusherClient.SubscriptionEventHandler(delegate(object s)
            {
                Debug.WriteLine("subscribed");
                reset.Set();
            });

            Debug.WriteLine("binding");
            channel.Bind(eventName, delegate(dynamic data)
            {
                Debug.WriteLine("event received");
                eventReceived = true;
                reset.Set();
            });

            Debug.WriteLine("waiting to subscribe");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Debug.WriteLine("triggering");
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger(channelName, eventName, new { hello = "world" });

            Debug.WriteLine("waiting for event to be received");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(eventReceived);
        }

        [Test]
        public void it_can_trigger_an_event_with_a_percent_in_the_message()
        {
            var eventJSON = File.ReadAllText("AcceptanceTests/percent-message.json");
            var message = new JavaScriptSerializer().Deserialize(eventJSON, typeof(object));

            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
            ITriggerResult result = pusher.Trigger("my-channel", "my_event", message);
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

    [TestFixture]
    public class When_Triggering_an_Event_over_HTTPS
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
        }

        [Test]
        public void It_should_return_a_200_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions() { Encrypted = true } );
            ITriggerResult result = pusher.Trigger("my-channel", "my_event", new { hello = "world" });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void It_should_expose_a_single_event_id_when_publishing_to_a_single_channel()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions() { Encrypted = true });
            ITriggerResult result = pusher.Trigger("ch1", "my_event", new { hello = "world" });
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.AreEqual(1, result.EventIds.Count);
        }

        [Test]
        public void It_should_expose_a_multiple_event_ids_when_publishing_to_multiple_channels()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions() { Encrypted = true });
            var channels = new string[]{"ch1", "ch2", "ch3"};
            ITriggerResult result = pusher.Trigger(channels, "my_event", new { hello = "world" });
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch2"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch3"));
            Assert.AreEqual(channels.Length, result.EventIds.Count);
        }
    }
}
