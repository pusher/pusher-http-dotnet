using System;
using System.Configuration;
using System.Net;
using NUnit.Framework;

namespace PusherDotNet.Tests
{
	[TestFixture]
	public class PusherProviderTests
	{
		private PusherProvider _defaultProvider;

		public void SetupDefaultProvider()
		{
			var applicationId = ConfigurationManager.AppSettings["applicationId"];
			var applicationKey = ConfigurationManager.AppSettings["applicationKey"];
			var applicationSecret = ConfigurationManager.AppSettings["applicationSecret"];

			if (String.IsNullOrEmpty(applicationId))
				Assert.Fail("applicationId not specified in app.config appSettings");
			if (String.IsNullOrEmpty(applicationKey))
				Assert.Fail("applicationKey not specified in app.config appSettings");
			if (String.IsNullOrEmpty(applicationSecret))
				Assert.Fail("applicationSecret not specified in app.config appSettings");

			_defaultProvider = new PusherProvider(applicationId, applicationKey, applicationSecret);
		}

		[Test]
		[Explicit("Set your credentials in app.config for this test to pass")]
		public void CanTriggerPush()
		{
			SetupDefaultProvider();
			var request = new TestPusherRequest("test_channel", "my_event", @"{""some"":""data""}");

			_defaultProvider.Trigger(request);
		}

		[Test]
		[Explicit("Set your credentials in app.config for this test to pass")]
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