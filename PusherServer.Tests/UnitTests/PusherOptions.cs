using NSubstitute;
using NUnit.Framework;
using System;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_creating_a_new_PusherOptions_instance
    {
        [Test]
        public void a_default_RestClient_should_be_used_if_one_is_not_set_on_PusherOptions_parameter()
        {
            var options = new PusherOptions();
            Assert.IsNotNull(options.RestClient);
        }

        [Test]
        public void Host_defaults_to_api_pusherapp_com()
        {
            var options = new PusherOptions();
            Assert.AreEqual("api.pusherapp.com", options.Host);
        }

        [Test]
        public void scheme_is_stripped_from_value_when_setting_host()
        {
            var httpsOptions = new PusherOptions();
            httpsOptions.Host = "https://api.pusherapp.com";
            Assert.AreEqual("api.pusherapp.com", httpsOptions.Host);

            var httpOptions = new PusherOptions();
            httpOptions.Host = "http://api.pusherapp.com";
            Assert.AreEqual("api.pusherapp.com", httpOptions.Host);
        }
        
        [Test]
        public void Port_defaults_to_80()
        {
            var options = new PusherOptions();
            Assert.AreEqual(80, options.Port);
        }

        [Test]
        public void when_Encrypted_option_is_set_Port_is_changed_to_443()
        {
            var options = new PusherOptions() { Encrypted = true };
            Assert.AreEqual(443, options.Port);
        }

        [Test]
        public void when_Encrypted_option_is_set_Port_is_changed_to_443_unless_Port_has_already_been_modified()
        {
            var options = new PusherOptions() { Port = 90 };
            options.Encrypted = true;
            Assert.AreEqual(90, options.Port);
        }
    }
}