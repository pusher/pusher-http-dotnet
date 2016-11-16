using System;
using System.Net;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PusherServer.RestfulClient;
using RestSharp;
using RestSharp.Serializers;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_Triggering_an_Event
    {
        IPusher _pusher;
        IRestClient _subClient;
        IPusherRestClient _subPusherClient;

        readonly string _channelName = "my-channel";
        readonly string _eventName = "my_event";
        readonly object _eventData = new { hello = "world" };

        private IRestResponse V7_PROTOCOL_SUCCESSFUL_RESPONSE;
        private IRestResponse V8_PROTOCOL_SUCCESSFUL_RESPONSE;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            V7_PROTOCOL_SUCCESSFUL_RESPONSE = Substitute.For<IRestResponse>();
            V7_PROTOCOL_SUCCESSFUL_RESPONSE.Content = "{}";
            V7_PROTOCOL_SUCCESSFUL_RESPONSE.StatusCode = HttpStatusCode.OK;

            V8_PROTOCOL_SUCCESSFUL_RESPONSE = Substitute.For<IRestResponse>();
            V8_PROTOCOL_SUCCESSFUL_RESPONSE.Content = TriggerResultHelper.TRIGGER_RESPONSE_JSON;
            V8_PROTOCOL_SUCCESSFUL_RESPONSE.StatusCode = HttpStatusCode.OK;
        }

        [SetUp]
        public void Setup()
        {
            _subClient = Substitute.For<IRestClient>();
            _subPusherClient = Substitute.For<IPusherRestClient>();

            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient,
                PusherRestClient = _subPusherClient
            };

            Config.AppId = "test-app-id";
            Config.AppKey = "test-app-key";
            Config.AppSecret = "test-app-secret";

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            _subClient.Execute(Arg.Any<IRestRequest>()).Returns(V8_PROTOCOL_SUCCESSFUL_RESPONSE);
        }

        [Test]
        public void trigger_calls_are_made_over_HTTP_by_default()
        {
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestScheme("http", _subClient)
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

            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestScheme("https", _subClient)
                )
            );
        }

        private bool CheckRequestScheme(string urlPrefix, IRestClient client)
        {
            return client.BaseUrl.Scheme.StartsWith(urlPrefix);
        }

        [Test]
        public void trigger_calls_are_made_over_port_443_when_Encrypted_option_is_set()
        {
            IPusherOptions options = new PusherOptions()
            {
                RestClient = _subClient,
                Encrypted = true,
                HostName = Config.HttpHost
            };

            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, options);

            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestPort(443, _subClient)
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

            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestPort(expectedPort, _subClient)
                )
            );
        }

        private bool CheckRequestPort(int port, IRestClient subClient)
        {
            return subClient.BaseUrl.Port == port;
        }

        [Test]
        public void url_resource_is_in_expected_format()
        {
            _pusher.Trigger(_channelName, _eventName, _eventData);

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
            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsPayload(x, _channelName, _eventName, _eventData)
                )
            );
        }

        private bool CheckRequestContainsPayload(IRestRequest request, string channelName, string eventName, object eventData)
        {
            var serializer = new JsonSerializer();
            var expectedBody = new TriggerBody() {
                name = eventName,
                channels = new[]{channelName},
                data = serializer.Serialize(eventData)
            };

            var expected = serializer.Serialize(expectedBody);

            return request.Parameters[0].Type == ParameterType.RequestBody &&
                request.Parameters[0].ToString().Contains( expected );
        }

        [Test]
        public async void with_async_and_a_single_channel_the_request_is_made()
        {
            await _pusher.TriggerAsync(_channelName, _eventName, _eventData);

            _subPusherClient.Received().ExecutePostAsync(Arg.Any<IPusherRestRequest>());
        }

        [Test]
        public async void with_async_and_a_single_channel_and_trigger_options_the_request_is_made()
        {
            await _pusher.TriggerAsync(_channelName, _eventName, _eventData);

            _subPusherClient.Received().ExecutePostAsync(Arg.Any<IPusherRestRequest>());
        }

        [Test]
        public async void with_async_and_multiple_channels_the_request_is_made()
        {
            await _pusher.TriggerAsync(new[] { "fish", "pie" }, _eventName, _eventData);

            _subPusherClient.Received().ExecutePostAsync(Arg.Any<IPusherRestRequest>());
        }

        [Test]
        public async void with_async_and_multiple_channels_and_trigger_options_the_request_is_made()
        {
            await _pusher.TriggerAsync(new[] { "fish", "pie" }, _eventName, _eventData);

            _subPusherClient.Received().ExecutePostAsync(Arg.Any<IPusherRestRequest>());
        }

        [Test]
        public void on_a_single_channel_the_socket_id_parameter_should_be_present_in_the_querystring()
        {
            var expectedSocketId = "123.098";
            
            _pusher.Trigger(_channelName, _eventName, _eventData, new TriggerOptions()
                    {
                        SocketId = expectedSocketId
                    });

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }


        [Test]
        public async void on_a_single_channel_the_socket_id_parameter_should_be_present_in_the_querystring_async()
        {
            var expectedSocketId = "123.098";

            await _pusher.TriggerAsync(
                _channelName,
                _eventName,
                _eventData,
                new TriggerOptions()
                {
                    SocketId = expectedSocketId
                }
            );

            _subPusherClient.Received().ExecutePostAsync(Arg.Is<IPusherRestRequest>(
                x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }

        [Test]
        public void on_a_multiple_channels_the_socket_id_parameter_should_be_present_in_the_querystring()
        {
            var expectedSocketId = "123.456";

            _pusher.Trigger(new[]{ "my-channel", "my-channel-2" }, _eventName, _eventData, new TriggerOptions()
                    {
                        SocketId = expectedSocketId
                    });

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }

        [Test]
        public async void on_a_multiple_channels_the_socket_id_parameter_should_be_present_in_the_querystring_async()
        {
            var expectedSocketId = "123.456";

            await _pusher.TriggerAsync(
                    new[] { "my-channel", "my-channel-2" },
                    _eventName,
                    _eventData,
                    new TriggerOptions()
                    {
                        SocketId = expectedSocketId
                    }
                );

            _subPusherClient.Received().ExecutePostAsync(
                Arg.Is<IPusherRestRequest>(
                    x => CheckRequestContainsSocketIdParameter(x, expectedSocketId)
                )
            );
        }

        [Test]
        public void libary_name_header_is_set_with_trigger_request()
        {
            _pusher.Trigger(_channelName, _eventName, _eventData);

            _subClient.Received().Execute(
                Arg.Is<IRestRequest>(
                    x => CheckRequestContainsHeaderParameter(x, "Pusher-Library-Name", "pusher-http-dotnet")
                )
            );
        }

        [Test]
        public async void socket_id_cannot_contain_colon_prefix()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId(":444.444");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \":444.444\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_cannot_contain_colon_suffix()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId("444.444:");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"444.444:\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_cannot_contain_letters_suffix()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId("444.444a");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"444.444a\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_must_contain_a_period_point()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId("444");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"444\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_must_not_contain_newline_prefix()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId("\n444.444");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"\n444.444\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_must_not_contain_newline_suffix()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId("444.444\n");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"444.444\n\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void socket_id_must_not_be_empty_string()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithSocketId(string.Empty);
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("socket_id \"\" was not in the form: \\A\\d+\\.\\d+\\z", caughtException.Message);
        }

        [Test]
        public async void channel_must_not_have_trailing_colon()
		{
            FormatException caughtException = null;

            try
		    {
		        await TriggerWithChannelName("test_channel:");
		    }
		    catch (FormatException ex)
		    {
		        caughtException = ex;
		    }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("channel name \"test_channel:\" was not in the form: \\A[a-zA-Z0-9_=@,.;\\-]+\\z", caughtException.Message);
        }
		[Test]
		public async void channel_name_must_not_have_leading_colon()
		{
            FormatException caughtException = null;

            try
		    {
		        await TriggerWithChannelName(":test_channel");
		    }
		    catch (FormatException ex)
		    {
                caughtException = ex;
		    }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("channel name \":test_channel\" was not in the form: \\A[a-zA-Z0-9_=@,.;\\-]+\\z", caughtException.Message);
        }
		
        [Test]
		public async void channel_name_must_not_have_leading_colon_newline()
        {
            FormatException caughtException = null;

            try
            {
                await TriggerWithChannelName(":\ntest_channel");
            }
            catch (FormatException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("channel name \":\ntest_channel\" was not in the form: \\A[a-zA-Z0-9_=@,.;\\-]+\\z", caughtException.Message);
        }
		
        [Test]
        public async void channel_name_must_not_have_trailing_colon_newline()
        {
            FormatException caughtException = null;

		    try
		    {
		        await TriggerWithChannelName("test_channel\n:");
		    }
		    catch (FormatException ex)
		    {
		        caughtException = ex;
		    }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("channel name \"test_channel\n:\" was not in the form: \\A[a-zA-Z0-9_=@,.;\\-]+\\z", caughtException.Message);
        }
		
		[Test]
		public void channel_names_in_array_must_be_validated()
		{
		    FormatException caughtException = null;

		    try
		    {
		        _pusher.Trigger(new[] { "this_one_is_okay", "test_channel\n:" }, _eventName, _eventData);
		    }
		    catch (FormatException ex)
		    {
		        caughtException = ex;
		    }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("channel name \"test_channel\n:\" was not in the form: \\A[a-zA-Z0-9_=@,.;\\-]+\\z", caughtException.Message);
        }

        [Test]
        public async void channel_names_must_not_exceed_allowed_length()
        {
            ArgumentOutOfRangeException caughtException = null;

            try
            {
                var channelName = new string('a', ValidationHelper.CHANNEL_NAME_MAX_LENGTH + 1);
                await TriggerWithChannelName(channelName);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.AreEqualIgnoringCase("Specified argument was out of the range of valid values.\r\nParameter name: The length of the channel name was greater than the allowed 164 characters", caughtException.Message);
        }

        private async Task<TriggerResult2> TriggerWithSocketId(string socketId)
        {
            _pusher.Trigger(_channelName, _eventName, _eventData, new TriggerOptions()
            {
                SocketId = socketId
            });

            var response = await _pusher.TriggerAsync(_channelName, _eventName, _eventData);

            return response;
        }

        private async Task<TriggerResult2> TriggerWithChannelName(string channelName)
        {
            _pusher.Trigger(channelName, _eventName, _eventData);

            var response = await _pusher.TriggerAsync(channelName, _eventName, _eventData);

            return response;
        }

        private static bool CheckRequestContainsSocketIdParameter(IRestRequest request, string expectedSocketId) {
            var parameter = request.Parameters[0];
            return parameter.Type == ParameterType.RequestBody &&
                parameter.ToString().Contains("socket_id") &&
                parameter.ToString().Contains(expectedSocketId);
        }

        private static bool CheckRequestContainsSocketIdParameter(IPusherRestRequest request, string expectedSocketId)
        {
            var parameter = request.GetContentAsJsonString();
            return parameter.Contains("socket_id") &&
                   parameter.Contains(expectedSocketId);
        }

        private static bool CheckRequestContainsHeaderParameter(IRestRequest request, string headerName, string headerValue)
        {
            bool found = false;
            foreach(Parameter p in request.Parameters)
            {
                if(p.Type == ParameterType.HttpHeader &&
                   p.Name == headerName &&
                   p.Value.ToString() == headerValue)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}

