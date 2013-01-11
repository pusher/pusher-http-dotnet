using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Security.Cryptography;
using RestSharp.Serializers;
//using Newtonsoft.Json;

namespace Pusher.Server
{
    public class Pusher : IPusher
    {
        private static string REST_API_URL = "http://api.pusherapp.com";

        private string _appId;
        private string _appKey;
        private string _appSecret;
        private IRestClient _client;

        public Pusher(string appId, string appKey, string appSecret):
            this(appId, appKey, appSecret, null)

        {
        }

        public Pusher(string appId, string appKey, string appSecret, IPusherOptions options)
        {
            if (options != null)
            {
                _client = options.RestClient;
            }
            else
            {
                _client = new RestClient();
            }
            _client.BaseUrl = REST_API_URL;
            _appId = appId;
            _appKey = appKey;
            _appSecret = appSecret;
        }

        public ITriggerResult Trigger(string channelName, string eventName, object data)
        {
            var channelNames = new string[] { channelName };
            return Trigger(channelNames, eventName, data);
        }

        public ITriggerResult Trigger(string[] channelNames, string eventName, object data)
        {
            return Trigger(channelNames, eventName, data, new TriggerOptions());
        }

        public ITriggerResult Trigger(string channelName, string eventName, object data, ITriggerOptions options)
        {
            var channelNames = new string[] { channelName };
            return Trigger(channelNames, eventName, data, options);
        }

        public ITriggerResult Trigger(string[] channelNames, string eventName, object data, ITriggerOptions options)
        {
            Dictionary<string, string> additionalPostParams = new Dictionary<string, string>();

            var serializer = new JsonSerializer();

            TriggerBody bodyData = new TriggerBody()
            {
                name = "",
                data = serializer.Serialize(data),
                channels = channelNames
            };

            if (string.IsNullOrEmpty(options.SocketId) == false)
            {
                bodyData.socket_id = options.SocketId;
            }

            IRestResponse response = ExecuteRequest(channelNames, eventName, bodyData);
            TriggerResult result = new TriggerResult(response);
            return result;
        }

        public IAuthenticationSignature Authenticate(string channelName, string socketId)
        {
            return new AuthenticationSignature(this._appKey, this._appSecret, channelName, socketId);
        }

        public IAuthenticationSignature Authenticate(string channelName, string socketId, PresenceChannelData presenceData)
        {
            return new AuthenticationSignature(this._appKey, this._appSecret, channelName, socketId, presenceData);
        }

        private IRestResponse ExecuteRequest(string[] channelNames, string eventName, object requestBody)
        {
            var serializer = new JsonSerializer();

            var bodyDataJson = serializer.Serialize(requestBody);
            var bodyMD5 = CryptoHelper.GetMd5Hash(bodyDataJson);
            var resource = String.Format("/apps/{0}/events", this._appId);
            var queryString = String.Format(
                "auth_key={0}&" +
                "auth_timestamp={1}&" +
                "auth_version={2}&" +
                "body_md5={3}",
                this._appKey,
                (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds),
                "1.0",
                bodyMD5);

            string authToSign = String.Format("POST\n{0}\n{1}", resource, queryString);
            var authSignature = CryptoHelper.GetHmac256(_appSecret, authToSign);
            
            var requestUrl = resource + "?" + queryString + "&auth_signature=" + authSignature;            
            var request = new RestRequest(requestUrl);
            request.RequestFormat = DataFormat.Json;
            request.Method = Method.POST;
            request.AddBody(requestBody);

            IRestResponse response = _client.Execute(request);
            return response;
        }

        
    }
}
