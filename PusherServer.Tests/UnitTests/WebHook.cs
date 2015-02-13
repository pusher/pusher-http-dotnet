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
        static string validSignature = GenerateValidSignature(secret, validBody);

        [Test]
        public void the_WebHook_will_be_valid_if_all_params_are_as_expected()
        {
            var webHook = new WebHook(secret, validSignature, validBody);
            Assert.IsTrue(webHook.IsValid);
        }

        [Test]
        public void the_event_name_can_be_retrieved_from_the_WebHook()
        {
            var webHook = new WebHook(secret, validSignature, validBody);
            Assert.AreEqual("channel_occupied", webHook.Events[0]["name"]);
        }

        [Test]
        public void the_channel_name_can_be_retrieved_from_the_WebHook()
        {
            var webHook = new WebHook(secret, validSignature, validBody);
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

            var webHook = new WebHook(secret, validSignature, body);
            Assert.AreEqual("test_channel", webHook.Events[0]["channel"]);
            Assert.AreEqual("channel_occupied", webHook.Events[0]["name"]);
            
            Assert.AreEqual("test_channel2", webHook.Events[1]["channel"]);
            Assert.AreEqual("channel_vacated", webHook.Events[1]["name"]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void the_WebHook_will_throw_exception_if_secret_is_null()
        {
            var webHook = new WebHook(null, validSignature, validBody);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void the_WebHook_will_throw_exception_if_secret_is_empty()
        {
            var webHook = new WebHook(string.Empty, validSignature, validBody);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_signature_is_null()
        {
            var webHook = new WebHook(secret, null, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_signature_is_empty()
        {
            var webHook = new WebHook(secret, string.Empty, validBody);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_body_is_null()
        {
            var webHook = new WebHook(secret, validSignature, null);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_invalid_if_they_body_is_empty()
        {
            var webHook = new WebHook(secret, validSignature, string.Empty);
            Assert.IsFalse(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_will_be_valid_given_alternative_values()
        {
            var signature = "851f492bab8f7652a2e4c82cd0212d97b4e678edf085c06bf640ed45ee7b1169";
            var secret = "1c9c753dddfd049dd7f1";
            var body = "{\"time_ms\":1423778833207,\"events\":[{\"channel\":\"test_channel\",\"name\":\"channel_occupied\"}]}";

            var webHook = new WebHook(secret, signature, body);
            Assert.IsTrue(webHook.IsValid);
        }

        [Test]
        public void the_WebHook_time_in_ms_is_correctly_parsed()
        {
            var fakeMillis = "1423850522000";
            var expectedDate = new DateTime(2015, 2, 13, 18, 2, 2, DateTimeKind.Utc);
            var secret = "1c9c753dddfd049dd7f1";
            var body = "{\"time_ms\":" + fakeMillis + ",\"events\":[{\"channel\":\"test_channel\",\"name\":\"channel_occupied\"}]}";
            var expectedSignature = GenerateValidSignature(secret, body);

            var webHook = new WebHook(secret, expectedSignature, body);
            Assert.AreEqual(expectedDate, webHook.Time);
        }

    }
}
