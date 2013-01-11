using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RestSharp.Serializers;

namespace Pusher.Server.Tests.UnitTests
{
    [TestFixture]
    public class When_authenticating_a_private_channel
    {
        IPusher _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);

        [Test]
        public void the_auth_response_is_valid()
        {
            string channelName = "my-channel";
            string socketId = "some_socket_id";

            string expectedAuthString = Config.AppKey + ":" + CreateSignedString(channelName, socketId);

            IAuthenticationSignature result = _pusher.Authenticate(channelName, socketId);
            Assert.AreEqual(expectedAuthString, result.auth);
        }

        private string CreateSignedString(string channelName, string socketId)
        {
            var stringToSign = socketId + ":" + channelName;
            return CryptoHelper.GetHmac256(Config.AppSecret, stringToSign);
        }
    }

    [TestFixture]
    public class When_authenticating_a_presence_channel
    {
        IPusher _pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret);

        [Test]
        public void the_auth_response_is_valid()
        {
            string channelName = "my-channel";
            string socketId = "some_socket_id";

            var serializer = new JsonSerializer();

            PresenceChannelData data = new PresenceChannelData()
            {
                user_id = "unique_user_id",
                user_info = new { twitter_id = "@leggetter" }
            };
            string presenceJSON = serializer.Serialize(data);

            string expectedAuthString = Config.AppKey + ":" + CreateSignedString(channelName, socketId, presenceJSON);

            IAuthenticationSignature result = _pusher.Authenticate(channelName, socketId, data);
            Assert.AreEqual(expectedAuthString, result.auth);
        }

        private string CreateSignedString(string channelName, string socketId, string presenceJSON)
        {
            var stringToSign = socketId + ":" + channelName + ":" + presenceJSON;
            return CryptoHelper.GetHmac256(Config.AppSecret, stringToSign);
        }
    }
}
