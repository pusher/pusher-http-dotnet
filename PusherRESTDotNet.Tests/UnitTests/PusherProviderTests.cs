using System;
using System.Configuration;
using System.Net;
using NUnit.Framework;
using PusherRESTDotNet.Authentication;

namespace PusherRESTDotNet.Tests.UnitTests
{
	[TestFixture]
	public class PusherProviderTests
	{
        [Test]
        public void AuthenticationStringIsCorrectlyFormedForPrivateChannel()
        {
            var appId = "1000";
            var appKey = "myAppKey";
            var appSecret = "myAppSecret";
            var channelName = "private-channel";
            var socketId = "socket_id";
            var helper = new PusherAuthenticationHelper(appId, appKey, appSecret);
            var expected = helper.CreateAuthenticatedString(socketId, channelName);

            IPusherProvider provider = new PusherProvider(appId, appKey, appSecret);
            string auth = provider.Authenticate(channelName, socketId);

            Assert.IsNotNullOrEmpty(auth);
            Assert.AreEqual(expected, auth);
        }

        [Test]
        public void AuthenticationStringIsCorrectlyFormedForPresenceChannel()
        {
            var appId = "1000";
            var appKey = "myAppKey";
            var appSecret = "myAppSecret";
            var channelName = "presence-channel";
            var presenceChannelData = new PresenceChannelData()
            {
                user_id = "leggetter",
                user_info = new { name = "Phil Leggetter", twitter = "@leggetter" }
            };
            var socketId = "socket_id";
            var helper = new PusherAuthenticationHelper(appId, appKey, appSecret);
            string expected = helper.CreateAuthenticatedString(socketId, channelName, presenceChannelData);

            IPusherProvider provider = new PusherProvider(appId, appKey, appSecret);
            string auth = provider.Authenticate(channelName, socketId, presenceChannelData);

            Assert.IsNotNullOrEmpty(auth);
            Assert.AreEqual(expected, auth);
        }
	}
}