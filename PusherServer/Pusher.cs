using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Serializers;

namespace PusherServer
{
    public class Pusher : IPusher
    {
        private static string DEFAULT_REST_API_HOST = "api.pusherapp.com";

        private string _appId;
        private string _appKey;
        private string _appSecret;
        private IPusherOptions _options;

        public Pusher(string appId, string appKey, string appSecret):
            this(appId, appKey, appSecret, null)

        {
        }

        public Pusher(string appId, string appKey, string appSecret, IPusherOptions options)
        {
            ThrowArgumentExceptionIfNullOrEmpty(appId, "appId");
            ThrowArgumentExceptionIfNullOrEmpty(appKey, "appKey");
            ThrowArgumentExceptionIfNullOrEmpty(appSecret, "appSecret");

            if (options == null)
            {
                options = new PusherOptions();
            }

            _appId = appId;
            _appKey = appKey;
            _appSecret = appSecret;
            _options = options;
        }

        private void ThrowArgumentExceptionIfNullOrEmpty(string value, string argumentName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(string.Format("{0} cannot be null or empty", argumentName));
            }
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
                name = eventName,
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

        public IAuthenticationData Authenticate(string channelName, string socketId)
        {
            return new AuthenticationData(this._appKey, this._appSecret, channelName, socketId);
        }

        public IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData presenceData)
        {
            return new AuthenticationData(this._appKey, this._appSecret, channelName, socketId, presenceData);
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

            _options.RestClient.BaseUrl = GetBaseUrl(_options);
            
            var requestUrl = resource + "?" + queryString + "&auth_signature=" + authSignature;            
            var request = new RestRequest(requestUrl);
            request.RequestFormat = DataFormat.Json;
            request.Method = Method.POST;
            request.AddBody(requestBody);

            IRestResponse response = _options.RestClient.Execute(request);
            return response;
        }

        private string GetBaseUrl(IPusherOptions _options)
        {
            string baseUrl = (_options.Encrypted ? "https" : "http") + "://" +
                DEFAULT_REST_API_HOST +
                (_options.Port == 80 ? "" : ":" + _options.Port);
            return baseUrl;
        }

        
    }
}
