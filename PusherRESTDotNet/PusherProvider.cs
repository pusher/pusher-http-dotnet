using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using PusherRESTDotNet.Authentication;

namespace PusherRESTDotNet
{
	public class PusherProvider : IPusherProvider
	{
        private readonly PusherConfig _config;
		private readonly string _applicationId;
		private readonly string _applicationKey;
		private readonly string _applicationSecret;
        private PusherAuthenticationHelper _authHelper;

		public PusherProvider(string applicationId, string applicationKey, string applicationSecret, PusherConfig config)
		{
			if (String.IsNullOrEmpty(applicationId))
				throw new ArgumentNullException("applicationId");
			if (String.IsNullOrEmpty(applicationKey)) 
				throw new ArgumentNullException("applicationKey");
			if (String.IsNullOrEmpty(applicationSecret))
				throw new ArgumentNullException("applicationSecret");

            _config = config;

			_applicationSecret = applicationSecret;
			_applicationKey = applicationKey;
			_applicationId = applicationId;

            _authHelper = new PusherAuthenticationHelper(_applicationId, _applicationKey, _applicationSecret);
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
            var origin = this._config.Scheme + "://" + this._config.Host;
            if (this._config.Port != 80)
            {
                origin += ":" + this._config.Port;
            }
			var requestUrl = String.Format(origin + "{0}?{1}&auth_signature={2}",
											GetBaseUri(request),
											GetQueryString(request),
											GetHmac256(request));

		    using(var client = new WebClient())
		    {
		        client.Encoding = Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/json");
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


        public string Authenticate(string channelName, string socketId)
        {
            return _authHelper.CreateAuthenticatedString(socketId, channelName);
        }


        public string Authenticate(string channelName, string socketId, PresenceChannelData presenceChannelData)
        {
            return _authHelper.CreateAuthenticatedString(socketId, channelName, presenceChannelData);
        }
    }
}