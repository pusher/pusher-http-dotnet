using NSubstitute;
using NUnit.Framework;
using System;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_creating_a_new_Pusher_instance
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appId_cannot_be_null()
        {
            var pusher = new Pusher(null, "app-key", "app-secret");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appKey_cannot_be_null()
        {
            var pusher = new Pusher("app-id", null, "app-secret");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appSecret_cannot_be_null()
        {
            var pusher = new Pusher("app-id", "app-key", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appId_cannot_be_empty_string()
        {
            var pusher = new Pusher(string.Empty, "app-key", "app-secret");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appKey_cannot_empty_string()
        {
            var pusher = new Pusher("app-id", string.Empty, "app-secret");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void appSecret_cannot_be_empty_string()
        {
            var pusher = new Pusher("app-id", "app-key", string.Empty);
        }
    }
}