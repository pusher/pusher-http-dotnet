using System;
using System.Configuration;
using System.Net;
using NUnit.Framework;
using PusherRESTDotNet.Authentication;

namespace PusherRESTDotNet.Tests.AcceptanceTests
{
	[TestFixture]
	public class PusherProviderTests
	{
		private PusherProvider _defaultProvider;
        string applicationId = ConfigurationManager.AppSettings["applicationId"];
        string applicationKey = ConfigurationManager.AppSettings["applicationKey"];
        string applicationSecret = ConfigurationManager.AppSettings["applicationSecret"];

		public void SetupDefaultProvider()
		{		
			if (String.IsNullOrEmpty(applicationId))
				Assert.Fail("applicationId not specified in app.config appSettings");
			if (String.IsNullOrEmpty(applicationKey))
				Assert.Fail("applicationKey not specified in app.config appSettings");
			if (String.IsNullOrEmpty(applicationSecret))
				Assert.Fail("applicationSecret not specified in app.config appSettings");

			_defaultProvider = new PusherProvider(applicationId, applicationKey, applicationSecret);
		}

        [Test]
        public void CanProvideCustomConfig()
        {
            var provider = new PusherProvider(
                applicationId,
                applicationKey,
                applicationSecret,
                new PusherConfig()
                {
                    Host = "50.17.194.145",
                    Port = 82,
                    Scheme = "http"
                });
            var request = new SimplePusherRequest("test_channel", "my_event", @"{""some"":""data""}");

            provider.Trigger(request);
        }

		[Test]
		public void CanTriggerPush()
		{
			SetupDefaultProvider();
			var request = new TestPusherRequest("test_channel", "my_event", @"{""some"":""data""}");

			_defaultProvider.Trigger(request);
		}

		[Test]
		public void CanTriggerPushWithAnonymousObject()
		{
			SetupDefaultProvider();
			var request = new ObjectPusherRequest("test_channel", "my_event", new
			                                                                  	{
			                                                                  		some = "data"
			                                                                  	});

			_defaultProvider.Trigger(request);
		}

		[Test]
		[ExpectedException(typeof(WebException))]
		public void BlowsUpOnTriggerPushWithBadCredentials()
		{
			var request = new TestPusherRequest("test_channel", "my_event", @"{""some"":""data""}");

			var provider = new PusherProvider("meh", "foo", "bar");
			provider.Trigger(request);
		}

		public class TestPusherRequest : PusherRequest
		{
			private readonly string _jsonData;

			public TestPusherRequest(string channelName, string eventName, string jsonData)
				: base(channelName, eventName)
			{
				_jsonData = jsonData;
			}

			public override string JsonData
			{
				get { return _jsonData; }
			}
		}
	}
}