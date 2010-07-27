using System.Net;
using NUnit.Framework;

namespace PusherDotNet.Tests
{
	[TestFixture]
	public class PusherProviderTests
	{
		[Test]
		[Explicit("To get this to pass, set up the PusherProvider with your credentials")]
		public void CanTriggerPush()
		{
			var request = new PusherRequest("test_channel")
								{
								EventName = "my_event",
								JsonData = @"{""some"":""data""}"
							};

			var provider = new PusherProvider("[YOUR APP ID]", "[YOUR APP KEY]", "[YOUR APP SECRET]");
			provider.Trigger(request);
		}

		[Test]
		[ExpectedException(typeof(WebException))]
		public void BlowsUpOnTriggerPushWithBadCredentials()
		{
			var request = new PusherRequest("test_channel")
			{
				EventName = "my_event",
				JsonData = @"{""some"":""data""}"
			};

			var provider = new PusherProvider("meh", "foo", "bar");
			provider.Trigger(request);
		}
	}
}