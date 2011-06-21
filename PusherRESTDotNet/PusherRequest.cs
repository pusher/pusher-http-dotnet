using System;

namespace PusherRESTDotNet
{
	/// <summary>
	/// Abstract base class for pusher requests
	/// </summary>
	public abstract class PusherRequest
	{
		protected PusherRequest(string channelName, string eventName)
		{
			ChannelName = channelName;
			EventName = eventName;
		}

		public string EventName { get; private set; }
		public string ChannelName { get; private set; }
		public string SocketId { get; set; }

		public abstract string JsonData { get; }
	}
}