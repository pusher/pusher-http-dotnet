using NSubstitute;
using NUnit.Framework;
using PusherServer.RestfulClient;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_using_async_Get_to_retrieve_a_list_of_application_channels
    {
        IPusher _pusher;
        IPusherRestClient _subPusherClient;

        [SetUp]
        public void Setup()
        {
            _subPusherClient = Substitute.For<IPusherRestClient>();

            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subPusherClient
            };

            Config.AppId = "test-app-id";
            Config.AppKey = "test-app-key";
            Config.AppSecret = "test-app-secret";

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);
        }

        [Test]
        public async void url_is_in_expected_format()
        {
            await _pusher.GetAsync<object>("/channels");

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.ResourceUri.StartsWith("/apps/" + Config.AppId + "/channels")
                )
            );
        }

        [Test]
        public async void GET_request_is_made()
        {
            await _pusher.GetAsync<object>("/channels");

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.Method == PusherMethod.GET
                )
            );
        }

        [Test]
        public async void additional_parameters_should_be_added_to_query_string()
        {
            await _pusher.GetAsync<object>("/channels", new { filter_by_prefix = "presence-", info = "user_count" });

#pragma warning disable 4014
            _subPusherClient.Received().ExecuteGetAsync<object>(
#pragma warning restore 4014
                Arg.Is<IPusherRestRequest>(
                    x => x.ResourceUri.Contains("&filter_by_prefix=presence-") &&
                         x.ResourceUri.Contains("&info=user_count")
                )
            );
        }
    }
}
