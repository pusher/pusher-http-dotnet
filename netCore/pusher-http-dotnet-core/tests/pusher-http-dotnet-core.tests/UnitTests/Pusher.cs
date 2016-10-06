using NUnit.Framework;
using System;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_creating_a_new_Pusher_instance
    {
        [Test]
        public void appId_cannot_be_null()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher(null, "app-key", "app-secret");
            });
        }

        [Test]
        public void appKey_cannot_be_null()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher("app-id", null, "app-secret");
            });
        }

        [Test]
        public void appSecret_cannot_be_null()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher("app-id", "app-key", null);
            });
        }

        [Test]
        public void appId_cannot_be_empty_string()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher(string.Empty, "app-key", "app-secret");
            });
        }

        [Test]
        public void appKey_cannot_empty_string()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher("app-id", string.Empty, "app-secret");
            });
        }

        [Test]
        public void appSecret_cannot_be_empty_string()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Pusher("app-id", "app-key", string.Empty);
            });
        }
    }
}