using NSubstitute;
using NUnit.Framework;
using RestSharp;

namespace Pusher.Server.Tests.UnitTests
{
    [TestFixture]
    public class When_Triggering_an_Event_with_socketId
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

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);
        }

        [Test]
        public void on_a_single_channel_the_socket_id_parameter_should_be_present_in_the_querystring()
        {
            var expectedSocketId = "some_socket_id";
            
            ITriggerResult result =
                _pusher.Trigger(
                    "my-channel",
                    "my_event",
                    new { hello = "world" },
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
                    "my_event",
                    new { hello = "world" },
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
