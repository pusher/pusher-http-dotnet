using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class when_creating_a_webhook
    {
        private static string GenerateValidSignature(string secret, string stringToSign)
        {
            return CryptoHelper.GetHmac256(secret, stringToSign);
        }

        static string secret = "some_crazy_secret";

        static string validBody = "{\"time_ms\": 1327078148132, \"events\": [{\"name\": \"channel_occupied\", \"channel\": \"test_channel\" }]}";
        static string validContentType = "application/json";
        static string validSignature = GenerateValidSignature(secret, validBody);

        [Test]
        public void the_WebHook_will_be_valid_if_all_params_are_as_expected()
        {
            var webHook = new WebHook(secret, validSignature, validContentType, validBody);
            Assert.IsTrue(webHook.IsValid);
        }

        [Test]
        public void the_event_name_can_be_retrieved_from_the_WebHook()
        {
            var webHook = new WebHook(secret, validSignature, validContentType, validBody);
            Assert.AreEqual("channel_occupied", webHook.Events[0]["name"]);
        }

        [Test]
        public void the_channel_name_can_be_retrieved_from_the_WebHook()
        {
            var webHook = new WebHook(secret, validSignature, validContentType, validBody);
            Assert.AreEqual("test_channel", webHook.Events[0]["channel"]);
        }

        [Test]
        public void the_WebHook_can_contain_multiple_events()
        {
            var body = "{\"time_ms\": 1327078148132, \"events\": " +
                "[" +
                    "{\"name\": \"channel_occupied\", \"channel\": \"test_channel\" }," +
                    "{\"name\": \"channel_vacated\", \"channel\": \"test_channel2\" }" +
                "]}";

            var webHook = new WebHook(secret, validSignature, validContentType, body);
            Assert.AreEqual("test_channel", webHook.Events[0]["channel"]);
            Assert.AreEqual("channel_occupied", webHook.Events[0]["name"]);
            
            Assert.AreEqual("test_channel2", webHook.Events[1]["channel"]);
            Assert.AreEqual("channel_vacated", webHook.Events[1]["name"]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void the_WebHook_will_throw_exception_if_secret_is_null()
        {
            var webHook = new WebHook(null, validSignature, validContentType, validBody);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void the_WebHook_will_throw_exception_if_secret_is_empty()
        {
            var webHook = new WebHook(string.Empty, validSignature, validContentType, validBody);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_signature_is_null()
        {
            var webHook = new WebHook(secret, null, validContentType, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_signature_is_empty()
        {
            var webHook = new WebHook(secret, string.Empty, validContentType, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_contentType_is_null()
        {
            var webHook = new WebHook(secret, validSignature, null, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_contentType_is_empty()
        {
            var webHook = new WebHook(secret, validSignature, string.Empty, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_body_is_null()
        {
            var webHook = new WebHook(secret, validSignature, validContentType, null);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_body_is_empty()
        {
            var webHook = new WebHook(secret, validSignature, validContentType, string.Empty);
            Assert.IsFalse(webHook.IsValid);
        }

    }
}
