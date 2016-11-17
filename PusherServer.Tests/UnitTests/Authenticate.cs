using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_authenticating_a_private_channel
    {
        IPusher _pusher;

        [TestFixtureSetUp]
        public void Setup()
        {
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
        }

        [Test]
        public void the_auth_response_is_valid()
        {
            string channelName = "my-channel";
            string socketId = "123.456";

            string expectedAuthString = Config.AppKey + ":" + CreateSignedString(channelName, socketId);

            IAuthenticationData result = _pusher.Authenticate(channelName, socketId);
            Assert.AreEqual(expectedAuthString, result.auth);
        }

        [Test]
        [ExpectedException]
        public void socket_id_cannot_contain_colon_prefix()
        {
            _pusher.Authenticate("private-test", ":444.444");
        }

        [Test]
        [ExpectedException]
        public void socket_id_cannot_contain_colon_suffix()
        {
            _pusher.Authenticate("private-test", "444.444:");
        }

        [Test]
        [ExpectedException]
        public void socket_id_cannot_contain_letters_suffix()
        {
            _pusher.Authenticate("private-test", "444.444a");
        }

        [Test]
        [ExpectedException]
        public void socket_id_must_contain_a_period_point()
        {
            _pusher.Authenticate("private-test", "444");
        }

        [Test]
        [ExpectedException]
        public void socket_id_must_not_contain_newline_prefix()
        {
            _pusher.Authenticate("private-test", "\n444.444");
        }

        [Test]
        [ExpectedException]
        public void socket_id_must_not_contain_newline_suffix()
        {
            _pusher.Authenticate("private-test", "444.444\n");
        }

        [Test]
        [ExpectedException]
        public void socket_id_must_not_be_empty_string()
        {
            _pusher.Authenticate("private-test", string.Empty);
        }

        [Test]
        [ExpectedException]
        public void channel_must_not_have_trailing_colon()
        {
            AuthWithChannelName("private-channel:");
        }
        [Test]
        [ExpectedException]
        public void channel_name_must_not_have_leading_colon()
        {
            AuthWithChannelName(":private-channel");
        }

        [Test]
        [ExpectedException]
        public void channel_name_must_not_have_leading_colon_newline()
        {
            AuthWithChannelName(":\nprivate-channel");
        }

        [Test]
        [ExpectedException]
        public void channel_name_must_not_have_trailing_colon_newline()
        {
            AuthWithChannelName("private-channel\n:");
        }

        [Test]
        [ExpectedException]
        public void channel_names_must_not_exceed_allowed_length()
        {
            var channelName = new String('a', ValidationHelper.CHANNEL_NAME_MAX_LENGTH + 1);
            AuthWithChannelName(channelName);
        }

        private void AuthWithChannelName(string channelName)
        {
            _pusher.Authenticate(channelName, "123.456");
        }

        private string CreateSignedString(string channelName, string socketId)
        {
            // null for presence data
            var stringToSign = socketId + ":" + channelName;
            return CryptoHelper.GetHmac256(Config.AppSecret, stringToSign);
        }
    }

    [TestFixture]
    public class When_authenticating_a_presence_channel
    {
        private IPusher _pusher;

        [TestFixtureSetUp]
        public void Setup()
        {
            _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void null_presence_data_throw_Exception()
        {
            string channelName = "my-channel";
            string socketId = "some_socket_id";

            _pusher.Authenticate(channelName, socketId, null);
        }

        [Test]
        public void the_auth_response_is_valid()
        {
            string channelName = "my-channel";
            string socketId = "123.456";

            PresenceChannelData data = new PresenceChannelData()
            {
                user_id = "unique_user_id",
                user_info = new { twitter_id = "@leggetter" }
            };
            string presenceJson = JsonConvert.SerializeObject(data);

            string expectedAuthString = Config.AppKey + ":" + CreateSignedString(channelName, socketId, presenceJson);

            IAuthenticationData result = _pusher.Authenticate(channelName, socketId, data);
            Assert.AreEqual(expectedAuthString, result.auth);
        }

        [Test]
        public void channel_data_is_encoded_as_JSON()
        {
            string channelName = "my-channel";
            string socketId = "123.456";

            PresenceChannelData data = new PresenceChannelData()
            {
                user_id = "unique_user_id",
                user_info = new { twitter_id = "@leggetter" }
            };

            string expectedChannelData = JsonConvert.SerializeObject(data);

            IAuthenticationData result = _pusher.Authenticate(channelName, socketId, data);
            Assert.AreEqual(expectedChannelData, result.channel_data);
        }

        private string CreateSignedString(string channelName, string socketId, string presenceJson)
        {
            var stringToSign = socketId + ":" + channelName + ":" + presenceJson;
            return CryptoHelper.GetHmac256(Config.AppSecret, stringToSign);
        }
    }
}
