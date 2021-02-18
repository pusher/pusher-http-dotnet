using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_application_channels_are_queried
    {
        [OneTimeSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
        }

        [Test]
        public async Task It_should_return_a_200_response()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<object> result = await pusher.GetAsync<object>("/channels");

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public async Task It_should_be_possible_to_deserialize_the_request_result_body_as_an_object()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<object> result = await pusher.GetAsync<object>("/channels");

            Assert.NotNull(result.Data);
        }

        [Test]
        public async Task It_should_be_possible_to_deserialize_the_a_channels_result_body_as_an_ChannelsList()
        {
            IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });

            IGetResult<ChannelsList> result = await pusher.GetAsync<ChannelsList>("/channels");

            Assert.IsTrue(result.Data.Channels.Count >= 0);
        }
    }
}
