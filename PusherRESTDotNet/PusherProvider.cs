using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using PusherRESTDotNet.Authentication;
using PusherServer;
using Newtonsoft.Json;

namespace PusherRESTDotNet
{
	public class PusherProvider : IPusherProvider
	{
		private IPusher _pusher;

		public PusherProvider(string applicationId, string applicationKey, string applicationSecret, PusherConfig config)
		{
			if (String.IsNullOrEmpty(applicationId))
				throw new ArgumentNullException("applicationId");
			if (String.IsNullOrEmpty(applicationKey)) 
				throw new ArgumentNullException("applicationKey");
			if (String.IsNullOrEmpty(applicationSecret))
				throw new ArgumentNullException("applicationSecret");

			_pusher = new Pusher(applicationId, applicationKey, applicationSecret);
		}

		public PusherProvider(string applicationId, string applicationKey, string applicationSecret):
			this(applicationId, applicationKey, applicationSecret, new PusherConfig())
		{
		}

		/// <summary>
		/// Sends the request. Will throw a WebException if anything other than a 202 response is encountered
		/// </summary>
		/// <param name="request"></param>
		public void Trigger(PusherRequest request)
		{
			var result = _pusher.Trigger(request.ChannelName, request.EventName, request.JsonData, new TriggerOptions()
			{
				SocketId = request.SocketId
			});

			if (result.StatusCode != HttpStatusCode.OK)
			{
				throw new WebException("The Trigger did no succeed: " + result.Body);
			}
		}

		public string Authenticate(string channelName, string socketId)
		{
			IAuthenticationData signature = _pusher.Authenticate(channelName, socketId);
			return JsonConvert.SerializeObject(signature);
		}


		public string Authenticate(string channelName, string socketId, PusherRESTDotNet.Authentication.PresenceChannelData presenceChannelData)
		{
			PusherServer.PresenceChannelData data = new PusherServer.PresenceChannelData()
			{
				user_id = presenceChannelData.user_id,
				user_info = presenceChannelData.user_info
			};
			IAuthenticationData signature = _pusher.Authenticate(channelName, socketId, data);
			return JsonConvert.SerializeObject(signature);
		}
	}
}