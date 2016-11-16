using NSubstitute;
using NUnit.Framework;
using PusherServer.RestfulClient;
using RestSharp;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_using_Get_to_retrieve_a_list_of_application_channels
    {
        IPusher _pusher;
        IRestClient _subClient;

        [SetUp]
        public void Setup()
        {
            _subClient = Substitute.For<IRestClient>();

            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient
            };

            Config.AppId = "test-app-id";
            Config.AppKey = "test-app-key";
            Config.AppSecret = "test-app-secret";

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);
        }

        [Test]
        public void url_is_in_expected_format()
        {
            _pusher.Get<object>("/channels");

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => x.Resource.StartsWith("/apps/" + Config.AppId + "/channels")
                )
            );
        }

        [Test]
        public void GET_request_is_made()
        {
            _pusher.Get<object>("/channels");

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => x.Method == Method.GET
                )
            );
        }

        [Test]
        public void additional_parameters_should_be_added_to_query_string()
        {
            _pusher.Get<object>("/channels", new { filter_by_prefix = "presence-", info = "user_count" });

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => x.Resource.Contains("&filter_by_prefix=presence-") &&
                         x.Resource.Contains("&info=user_count")
                )
            );
        }
    }

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
                PusherRestClient = _subPusherClient
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
