namespace PusherRESTDotNet
{
	public class SimplePusherRequest : PusherRequest
	{
		private readonly string _jsonData;

		public SimplePusherRequest(string channelName, string eventName, string jsonData)
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