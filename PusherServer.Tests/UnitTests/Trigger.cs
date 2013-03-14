using NSubstitute;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serializers;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_Triggering_an_Event
    {
        IPusher _pusher;
        IRestClient _subClient;

        string channelName = "my-channel";
        string eventName = "my_event";
        object eventData = new { hello = "world" };

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
        public void trigger_calls_are_made_over_HTTP_by_default()
        {
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestIsMadeOver("http://", _subClient, x)
                )
            );
        }

        [Test]
        public void trigger_calls_are_made_over_HTTPS_when_Encrypted_option_is_set()
        {
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient,
                Encrypted = true
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestIsMadeOver("https://", _subClient, x)
                )
            );
        }

        private bool CheckRequestIsMadeOver(string urlPrefix, IRestClient client, IRestRequest req)
        {
            return client.BaseUrl.StartsWith(urlPrefix);
        }

        [Test]
        public void trigger_calls_are_made_over_port_443_when_Encrypted_option_is_set()
        {
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient,
                Encrypted = true
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => _CheckRequestPort(443, _subClient, x)
                )
            );
        }

        [Test]
        public void trigger_calls_are_made_over_configured_Port_when_option_is_set()
        {
            int expectedPort = 900;
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient,
                Port = expectedPort
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => _CheckRequestPort(expectedPort, _subClient, x)
                )
            );
        }

        private bool _CheckRequestPort(int port, IRestClient _subClient, IRestRequest x)
        {
            return _subClient.BaseUrl.EndsWith(":" + port);
        }

        [Test]
        public void url_resource_is_in_expected_format()
        {
            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestHasExpectedUrl(x)
                )
            );
        }

        private bool CheckRequestHasExpectedUrl(IRestRequest req)
        { 
            return req.Resource.StartsWith( "/apps/" + Config.AppId + "/events?");
        }

        [Test]
        public void post_payload_contains_channelName_eventName_and_eventData()
        {
            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsPayload(x, channelName, eventName, eventData)
                )
            );
        }

        private bool CheckRequestContainsPayload(IRestRequest request, string channelName, string eventName, object eventData)
        {
            var serializer = new JsonSerializer();
            var expectedBody = new TriggerBody() {
                name = eventName,
                channels = new string[]{channelName},
                data = serializer.Serialize(eventData)
            };
            var expected = serializer.Serialize(expectedBody);

            return request.Parameters[0].Type == ParameterType.RequestBody &&
                request.Parameters[0].ToString().Contains( expected );
        }

        [Test]
        public void on_a_single_channel_the_socket_id_parameter_should_be_present_in_the_querystring()
        {
            var expectedSocketId = "some_socket_id";
            
            ITriggerResult result =
                _pusher.Trigger(
                    channelName,
                    eventName,
                    eventData,
                    new TriggerOptions()
                    {
                        SocketId = expectedSocketId
                    }
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }

        [Test]
        public void on_a_multiple_channels_the_socket_id_parameter_should_be_present_in_the_querystring()
        {
            var expectedSocketId = "some_socket_id";

            ITriggerResult result =
                _pusher.Trigger(
                    new string[]{ "my-channel", "my-channel-2" },
                    eventName,
                    eventData,
                    new TriggerOptions()
                    {
                        SocketId = expectedSocketId
                    }
                );

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }

        private static bool CheckRequestContainsSocketIdParameter(IRestRequest request, string expectedSocketId) {
            var parameter = request.Parameters[0];
            return parameter.Type == ParameterType.RequestBody &&
                parameter.ToString().Contains("socket_id");
        }
    }
}
