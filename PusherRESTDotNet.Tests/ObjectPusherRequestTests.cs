using NUnit.Framework;

namespace PusherRESTDotNet.Tests
{
	[TestFixture]
	public class ObjectPusherRequestTests
	{
		[Test]
		public void SerializesObjectToJsonData()
		{
			var request = new ObjectPusherRequest("test_channel", "my_event", new { some = "data" });

			Assert.That(request.JsonData, Is.EqualTo(@"{""some"":""data""}"));
		}
	}
}