using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PusherRESTDotNet
{
	public class PusherProvider : IPusherProvider
	{
		private const string _host = "api.pusherapp.com";
		private readonly string _applicationId;
		private readonly string _applicationKey;
		private readonly string _applicationSecret;

		public PusherProvider(string applicationId, string applicationKey, string applicationSecret)
		{
			if (String.IsNullOrEmpty(applicationId))
				throw new ArgumentNullException("applicationId");
			if (String.IsNullOrEmpty(applicationKey)) 
				throw new ArgumentNullException("applicationKey");
			if (String.IsNullOrEmpty(applicationSecret))
				throw new ArgumentNullException("applicationSecret");

			_applicationSecret = applicationSecret;
			_applicationKey = applicationKey;
			_applicationId = applicationId;
		}

		/// <summary>
		/// Sends the request. Will throw a WebException if anything other than a 202 response is encountered
		/// </summary>
		/// <param name="request"></param>
		public void Trigger(PusherRequest request)
		{
			var requestUrl = String.Format("http://{0}{1}?{2}&auth_signature={3}",
											_host,
											GetBaseUri(request),
											GetQueryString(request),
											GetHmac256(request));

		    using(var client = new WebClient())
		    {
		        client.Encoding = Encoding.UTF8;
		        client.UploadString(requestUrl, request.JsonData);
		    }
		}

		private string GetBaseUri(PusherRequest request)
		{
			return String.Format("/apps/{0}/channels/{1}/events", _applicationId, request.ChannelName);
		}

		private string GetQueryString(PusherRequest request)
		{
			var output = String.Format("auth_key={0}&auth_timestamp={1}&auth_version=1.0&body_md5={2}&name={3}",
										_applicationKey,
										(int)((DateTime.UtcNow - new DateTime(1970,1,1)).TotalSeconds),
										GetMd5Hash(request.JsonData),
										request.EventName);

			return String.IsNullOrEmpty(request.SocketId) ? output : String.Format("{0}&socket_id={1}", output, request.SocketId);
		}

		private static string GetMd5Hash(string jsonData)
		{
			var hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(jsonData));
			return BytesToHex(hash);
		}

		private string GetHmac256(PusherRequest request)
		{
			var data = String.Format("POST\n{0}\n{1}", GetBaseUri(request), GetQueryString(request));
			var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_applicationSecret));
			var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));

			return BytesToHex(hash);
		}

		private static string BytesToHex(IEnumerable<byte> byteArray)
		{
			return String.Concat(byteArray.Select(bytes => bytes.ToString("x2")).ToArray());
		}
	}
}