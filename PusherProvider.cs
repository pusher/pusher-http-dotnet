using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PusherDotNet
{
	public class PusherProvider
	{
		private readonly string _host;
		private readonly string _applicationId;
		private readonly string _applicationKey;
		private readonly string _applicationSecret;

		public void PushIt(PusherRequest request)
		{
			var client = new WebClient
			             	{
								BaseAddress = GetBaseUri(request),
								QueryString = BuildQueryStringCollection(request),
			             	};
		}

		private NameValueCollection BuildQueryStringCollection(PusherRequest request)
		{
			var pairs = new NameValueCollection
			       	{
			       		{"auth_key", _applicationKey},
			       		{"auth_timestamp", _applicationKey},
			       		{"auth_version", "1.0"},
			       		{"body_md5", GetMd5Hash(request.JsonData)},
			       		{"name", request.EventName},
			       	};

			if(!String.IsNullOrEmpty(request.SocketId))
			{
				pairs.Add("socket_id", request.SocketId);
			}

			return pairs;
		}

		private static string GetMd5Hash(string jsonData)
		{
			return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(jsonData)));
		}

		private string GetBaseUri(PusherRequest request)
		{
			return String.Format("/apps/{0}/channels/{1}/events", _applicationId, request.ChannelName);
		}
	}

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


