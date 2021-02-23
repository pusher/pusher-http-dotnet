using Newtonsoft.Json;
using NUnit.Framework;
using PusherServer.Exceptions;
using PusherServer.Tests.Helpers;
using PusherServer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_Triggering_an_Event_on_a_single_Channel
    {
        IPusher _pusher;

        [OneTimeSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                TraceLogger = new DebugTraceLogger(),
            });
        }

        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            ITriggerResult asyncResult = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" });

            Assert.AreEqual(HttpStatusCode.OK, asyncResult.StatusCode);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task it_should_expose_the_event_id_async()
        {
            var waiting = new AutoResetEvent(false);

            ITriggerResult asyncResult = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" });

            waiting.Set();

            Assert.IsFalse(string.IsNullOrEmpty(asyncResult.EventIds["my-channel"]));
        }

        [Test]
        public async Task It_should_be_received_by_a_client_async()
        {
            string channelName = "my_channel";
            string eventName = "my_event";

            bool eventReceived = false;
            AutoResetEvent reset = new AutoResetEvent(false);

            var client = new PusherClient.Pusher(Config.AppKey)
            {
                Host = Config.WebSocketHost,
            };
            client.Connected += new PusherClient.ConnectedEventHandler(delegate (object sender)
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
            channel.Subscribed += new PusherClient.SubscriptionEventHandler(delegate (object s)
            {
                Debug.WriteLine("subscribed");
                reset.Set();
            });

            Debug.WriteLine("waiting for Subscribed");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Debug.WriteLine("binding");
            channel.Bind(eventName, delegate (dynamic data)
            {
                Debug.WriteLine("event received");
                eventReceived = true;
                reset.Set();
            });

            Debug.WriteLine("Bound. Triggering");
            await _pusher.TriggerAsync(channelName, eventName, new { hello = "world" });

            Debug.WriteLine("waiting for event to be received");
            reset.WaitOne(TimeSpan.FromSeconds(10));

            Assert.IsTrue(eventReceived);
        }

        [Test]
        public async Task it_can_trigger_an_event_with_a_percent_in_the_message_async()
        {
            string fileName = Path.Combine(Assembly.GetExecutingAssembly().Location, @"../../../../AcceptanceTests/percent-message.json");
            var eventJSON = File.ReadAllText(fileName);
            var message = JsonConvert.DeserializeObject(eventJSON);

            ITriggerResult result = await _pusher.TriggerAsync("my-channel", "my_event", message);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(EventDataSizeExceededException))]
        public async Task It_should_fail_for_an_event_data_size_greater_than_10KB_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
            });
            int size = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT + 1;
            List<Event> largeEvent = DataHelper.CreateEvents(numberOfEvents: 1, eventSizeInBytes: size);
            try
            {
                await pusher.TriggerAsync("my-channel", "my_event", largeEvent[0]);
            }
            catch(EventDataSizeExceededException e)
            {
                Assert.AreEqual("my-channel", e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual("my_event", e.EventName, nameof(e.EventName));
                throw;
            }
        }
    }

    [TestFixture]
    public class When_Triggering_an_Event_on_a_multiple_Channels
    {
        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            ITriggerResult asyncResult = await pusher.TriggerAsync(new string[] { "my-channel-1", "my-channel-2" }, "my_event", new { hello = "world" });

            Assert.AreEqual(HttpStatusCode.OK, asyncResult.StatusCode);
        }
    }

    [TestFixture]
    public class When_Triggering_a_Batch_of_Events
    {
        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
            });

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 9, eventSizeInBytes: 84);
            int size = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT;
            List<Event> largeEvent = DataHelper.CreateEvents(numberOfEvents: 1, eventSizeInBytes: size);
            events.AddRange(largeEvent);

            var result = await pusher.TriggerAsync(events.ToArray());

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(EventDataSizeExceededException))]
        public async Task It_should_fail_for_an_event_data_size_greater_than_10KB_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
            });

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 9, eventSizeInBytes: 84);
            int size = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT + 1;
            List<Event> largeEvent = DataHelper.CreateEvents(numberOfEvents: 1, eventSizeInBytes: size);
            events.AddRange(largeEvent);

            try
            {
                await pusher.TriggerAsync(events.ToArray());
            }
            catch (EventDataSizeExceededException e)
            {
                Assert.AreEqual("testChannel", e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual("testEvent", e.EventName, nameof(e.EventName));
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof(EventBatchSizeExceededException))]
        public async Task It_should_fail_for_a_batch_size_greater_than_10_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
            });

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 11, eventSizeInBytes: 92);
            await pusher.TriggerAsync(events.ToArray());
        }
    }

    [TestFixture]
    public class When_Triggering_an_Event_over_HTTPS
    {
        IPusher _pusher = null;

        [OneTimeSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                Encrypted = true,
                HostName = Config.HttpHost
            });
        }

        [Test]
        public async Task It_should_return_a_200_response()
        {
            ITriggerResult result = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" });
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task It_should_expose_a_single_event_id_when_publishing_to_a_single_channel()
        {
            ITriggerResult result = await _pusher.TriggerAsync("ch1", "my_event", new { hello = "world" });
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.AreEqual(1, result.EventIds.Count);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task It_should_expose_a_multiple_event_ids_when_publishing_to_multiple_channels()
        {
            var channels = new string[] { "ch1", "ch2", "ch3" };
            ITriggerResult result = await _pusher.TriggerAsync(channels, "my_event", new { hello = "world" });
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch2"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch3"));
            Assert.AreEqual(channels.Length, result.EventIds.Count);
        }
    }
}