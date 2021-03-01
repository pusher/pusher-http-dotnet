using Microsoft.VisualStudio.TestTools.UnitTesting;
using PusherServer.Tests.Helpers;
using System;

namespace PusherServer.Tests.UnitTests
{
    [TestClass]
    public class When_creating_a_new_Pusher_instance
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appId_cannot_be_null()
        {
            new Pusher(null, "app-key", "app-secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appKey_cannot_be_null()
        {
            new Pusher("app-id", null, "app-secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appSecret_cannot_be_null()
        {
            new Pusher("app-id", "app-key", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appId_cannot_be_empty_string()
        {
            new Pusher(string.Empty, "app-key", "app-secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appKey_cannot_empty_string()
        {
            new Pusher("app-id", string.Empty, "app-secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void appSecret_cannot_be_empty_string()
        {
            new Pusher("app-id", "app-key", string.Empty);
        }
    }
}