namespace PusherDotNet
{
	public class PusherRequest
	{
		public string EventName { get; set; }
		public string ChannelName { get; set; }
		public string JsonData { get; set; }
		public string SocketId { get; set; }
		
		public PusherRequest(string channelName)
		{
			ChannelName = channelName;
		}
	}
}