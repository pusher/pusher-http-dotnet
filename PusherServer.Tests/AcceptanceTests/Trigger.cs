using Newtonsoft.Json;
using NUnit.Framework;
using PusherServer.Exceptions;
using PusherServer.Tests.Helpers;
using PusherServer.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PusherServer.Tests.AcceptanceTests
{
    public class TestData
    {
        public TestData(string message)
        {
            Id = Guid.NewGuid();
            Message = message;
        }

        public Guid Id { get; set; }

        public string Message { get; set; }

        public void Validate(TestData actual)
        {
            Assert.IsNotNull(actual, nameof(TestData));
            Assert.AreEqual(Id, actual.Id, nameof(TestData.Id));
            Assert.AreEqual(Message, actual.Message, nameof(TestData.Message));
        }
    }

    [TestFixture]
    public class When_triggering_an_event_on_a_single_channel
    {
        IPusher _pusher;

        [OneTimeSetUp]
        public void Setup()
        {
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                TraceLogger = new DebugTraceLogger(),
            });
        }

        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            ITriggerResult asyncResult = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" }).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, asyncResult.StatusCode);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task It_should_expose_the_event_id_async()
        {
            ITriggerResult asyncResult = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" }).ConfigureAwait(false);

            Assert.IsFalse(string.IsNullOrEmpty(asyncResult.EventIds["my-channel"]));
        }

        [Test]
        public async Task It_should_be_received_by_a_client_async()
        {
            TestData actual = null;
            TestData expected = new TestData("Hello World!");
            string channelName = "my_channel";
            string eventName = "my_event";

            AutoResetEvent eventReceived = new AutoResetEvent(false);

            var client = new PusherClient.Pusher(Config.AppKey, new PusherClient.PusherOptions
            {
                Cluster = Config.Cluster,
                TraceLogger = new PusherClient.TraceLogger(),
            });

            await client.ConnectAsync().ConfigureAwait(false);

            var channel = await client.SubscribeAsync(channelName).ConfigureAwait(false);

            channel.Bind(eventName, delegate (PusherClient.PusherEvent data)
            {
                actual = JsonConvert.DeserializeObject<TestData>(data.Data);
                eventReceived.Set();
            });

            await _pusher.TriggerAsync(channelName, eventName, expected).ConfigureAwait(false);

            Assert.IsTrue(channel.IsSubscribed, nameof(channel.IsSubscribed));
            Assert.IsTrue(eventReceived.WaitOne(TimeSpan.FromSeconds(5)), "Expected to receive an event");
            expected.Validate(actual);
        }

        [Test]
        public async Task It_can_trigger_an_event_with_a_percent_in_the_message_async()
        {
            string fileName = Path.Combine(Assembly.GetExecutingAssembly().Location, @"../../../../AcceptanceTests/percent-message.json");
            var eventJSON = File.ReadAllText(fileName);
            var message = JsonConvert.DeserializeObject(eventJSON);

            ITriggerResult result = await _pusher.TriggerAsync("my-channel", "my_event", message).ConfigureAwait(false);
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
                await pusher.TriggerAsync(largeEvent[0].Channel, largeEvent[0].EventName, largeEvent[0]).ConfigureAwait(false);
            }
            catch(EventDataSizeExceededException e)
            {
                Assert.AreEqual(largeEvent[0].Channel, e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual(largeEvent[0].EventName, e.EventName, nameof(e.EventName));
                throw;
            }
        }
    }

    [TestFixture]
    public class When_triggering_an_encrypted_event_on_a_single_channel
    {
        IPusher _pusher;

        [OneTimeSetUp]
        public void Setup()
        {
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions
            {
                Cluster = Config.Cluster,
                EncryptionMasterKey = DataHelper.GenerateEncryptionMasterKey(),
                Encrypted = true,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
                TraceLogger = new DebugTraceLogger(),
            });
        }

        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            string channelName = "private-encrypted-channel";
            string eventName = "my-encrypted-event";
            ITriggerResult asyncResult = await _pusher.TriggerAsync(channelName, eventName, new TestData("Hello World!")).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, asyncResult.StatusCode);
        }

        [Test]
        public async Task It_should_be_received_by_a_client_async()
        {
            string channelName = "private-encrypted-channel";
            string eventName = "my-encrypted-event";
            string eventData = null;

            AutoResetEvent eventReceived = new AutoResetEvent(false);

            var client = new PusherClient.Pusher(Config.AppKey, new PusherClient.PusherOptions
            {
                Cluster = Config.Cluster,
                TraceLogger = new PusherClient.TraceLogger(),
                ChannelAuthorizer = new InMemoryChannelAuthorizer(_pusher as Pusher),
            });

            await client.ConnectAsync().ConfigureAwait(false);

            var channel = await client.SubscribeAsync(channelName).ConfigureAwait(false);

            channel.Bind(eventName, delegate (PusherClient.PusherEvent data)
            {
                eventData = data.Data;
                eventReceived.Set();
            });

            var testData = new TestData("Hello World!");
            await _pusher.TriggerAsync(channelName, eventName, testData).ConfigureAwait(false);

            Assert.IsTrue(channel.IsSubscribed, nameof(channel.IsSubscribed));
            Assert.IsTrue(eventReceived.WaitOne(TimeSpan.FromSeconds(5)), "Expected to receive an event");
            Assert.IsNotNull(eventData, nameof(eventData));
            Assert.IsTrue(eventData.Contains(testData.Message), testData.Message);
        }

        [Test]
        [ExpectedException(typeof(EventDataSizeExceededException))]
        public async Task It_should_fail_for_an_event_data_size_greater_than_10KB_async()
        {
            string channelName = "private-encrypted-channel";
            string eventName = "my-encrypted-event";
            int size = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT + 1;
            List<Event> largeEvent = DataHelper.CreateEvents(numberOfEvents: 1, eventSizeInBytes: size, channelId: channelName, eventId: eventName);
            try
            {
                await _pusher.TriggerAsync(largeEvent[0].Channel, largeEvent[0].EventName, largeEvent[0]).ConfigureAwait(false);
            }
            catch (EventDataSizeExceededException e)
            {
                Assert.AreEqual(largeEvent[0].Channel, e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual(largeEvent[0].EventName, e.EventName, nameof(e.EventName));
                throw;
            }
        }
    }

    [TestFixture]
    public class When_triggering_an_event_on_multiple_channels
    {
        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            ITriggerResult asyncResult = await pusher.TriggerAsync(new string[] { "my-channel-1", "my-channel-2" }, "my_event", new { hello = "world" }).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, asyncResult.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task It_should_fail_if_an_encrypted_channel_is_included_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            await pusher.TriggerAsync(new string[] { "my-channel-1", "private-encrypted-channel-2" }, "my_event", new { hello = "world" }).ConfigureAwait(false);
        }
    }

    [TestFixture]
    public class When_triggering_a_batch_of_events
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

            var result = await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);

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
                await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);
            }
            catch (EventDataSizeExceededException e)
            {
                Assert.AreEqual(largeEvent[0].Channel, e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual(largeEvent[0].EventName, e.EventName, nameof(e.EventName));
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
            await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);
        }
    }

    [TestFixture]
    public class When_triggering_an_encrypted_batch_of_events
    {
        [Test]
        public async Task It_should_return_a_200_response_async()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost,
                BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT,
                EncryptionMasterKey = DataHelper.GenerateEncryptionMasterKey(),
            });

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 9, eventSizeInBytes: 84, channelId: "private-encrypted-channel");
            var result = await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);

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
                EncryptionMasterKey = DataHelper.GenerateEncryptionMasterKey(),
            });

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 9, eventSizeInBytes: 84, channelId: "private-encrypted-channel");
            int size = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT + 1;
            List<Event> largeEvent = DataHelper.CreateEvents(numberOfEvents: 1, eventSizeInBytes: size, channelId: "private-encrypted-channel");
            events.AddRange(largeEvent);

            try
            {
                await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);
            }
            catch (EventDataSizeExceededException e)
            {
                Assert.AreEqual(largeEvent[0].Channel, e.ChannelName, nameof(e.ChannelName));
                Assert.AreEqual(largeEvent[0].EventName, e.EventName, nameof(e.EventName));
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

            List<Event> events = DataHelper.CreateEvents(numberOfEvents: 11, eventSizeInBytes: 92, channelId: "private-encrypted-channel");
            await pusher.TriggerAsync(events.ToArray()).ConfigureAwait(false);
        }
    }

    [TestFixture]
    public class When_triggering_an_event_over_https
    {
        IPusher _pusher = null;

        [OneTimeSetUp]
        public void Setup()
        {
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                Encrypted = true,
                HostName = Config.HttpHost
            });
        }

        [Test]
        public async Task It_should_return_a_200_response()
        {
            ITriggerResult result = await _pusher.TriggerAsync("my-channel", "my_event", new { hello = "world" }).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task It_should_expose_a_single_event_id_when_publishing_to_a_single_channel()
        {
            ITriggerResult result = await _pusher.TriggerAsync("ch1", "my_event", new { hello = "world" }).ConfigureAwait(false);
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.AreEqual(1, result.EventIds.Count);
        }

        [Test]
        [Ignore("This test requires a node that support batch triggers, which isn't available on the default")]
        public async Task It_should_expose_a_multiple_event_ids_when_publishing_to_multiple_channels()
        {
            var channels = new string[] { "ch1", "ch2", "ch3" };
            ITriggerResult result = await _pusher.TriggerAsync(channels, "my_event", new { hello = "world" }).ConfigureAwait(false);
            Assert.IsTrue(result.EventIds.ContainsKey("ch1"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch2"));
            Assert.IsTrue(result.EventIds.ContainsKey("ch3"));
            Assert.AreEqual(channels.Length, result.EventIds.Count);
        }
    }
}