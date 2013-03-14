using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Serializers;

namespace PusherServer
{
    /// <summary>
    /// Provides access to functionality within the Pusher service such as <see cref="Trigger"/> to trigger events
    /// and authenticating subscription requests to private and presence channels.
    /// </summary>
    public class Pusher : IPusher
    {
        private static string DEFAULT_REST_API_HOST = "api.pusherapp.com";

        private string _appId;
        private string _appKey;
        private string _appSecret;
        private IPusherOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pusher" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        public Pusher(string appId, string appKey, string appSecret):
            this(appId, appKey, appSecret, null)

        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pusher" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="options">Additional options to be used with the instance e.g. setting the call to the REST API to be made over HTTPS.</param>
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

        /// <summary>
        /// Triggers an event on the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <returns>The result of the call to the REST API</returns>
        public ITriggerResult Trigger(string channelName, string eventName, object data)
        {
            var channelNames = new string[] { channelName };
            return Trigger(channelNames, eventName, data);
        }

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelNames">The names of the channels the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <returns>The result of the call to the REST API</returns>
        public ITriggerResult Trigger(string[] channelNames, string eventName, object data)
        {
            return Trigger(channelNames, eventName, data, new TriggerOptions());
        }

        /// <summary>
        /// Triggers an event on the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions" />.</param>
        /// <returns>The result of the call to the REST API</returns>
        public ITriggerResult Trigger(string channelName, string eventName, object data, ITriggerOptions options)
        {
            var channelNames = new string[] { channelName };
            return Trigger(channelNames, eventName, data, options);
        }

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelNames"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions" />.</param>
        /// <returns>The result of the call to the REST API</returns>
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

            IRestResponse response = ExecuteTrigger(channelNames, eventName, bodyData);
            TriggerResult result = new TriggerResult(response);
            return result;
        }

        /// <summary>
        /// Authenticates the subscription request for a private channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <returns>
        /// Authentication data where the required authentication token can be accessed via <see cref="IAuthenticationData.auth" />
        /// </returns>
        public IAuthenticationData Authenticate(string channelName, string socketId)
        {
            return new AuthenticationData(this._appKey, this._appSecret, channelName, socketId);
        }

        /// <summary>
        /// Authenticates the specified channel name.
        /// </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="socketId">The socket id.</param>
        /// <param name="presenceData">The presence data.</param>
        /// <returns></returns>
        public IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData presenceData)
        {
            return new AuthenticationData(this._appKey, this._appSecret, channelName, socketId, presenceData);
        }

        private IRestResponse ExecuteTrigger(string[] channelNames, string eventName, object requestBody)
        {
           _options.RestClient.BaseUrl = GetBaseUrl(_options);

            var resource = String.Format("/apps/{0}/events", this._appId);
            var request = CreateAuthenticatedRequest("POST", resource, requestBody);

            IRestResponse response = _options.RestClient.Execute(request);
            return response;
        }

        private IRestRequest CreateAuthenticatedRequest(string requestType, string resource, object requestBody)
        {
            var queryString = String.Format(
                "auth_key={0}&" +
                "auth_timestamp={1}&" +
                "auth_version={2}",
                this._appKey,
                (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds),
                "1.0");

            if (requestBody != null)
            {
                var serializer = new JsonSerializer();
                var bodyDataJson = serializer.Serialize(requestBody);
                var bodyMD5 = CryptoHelper.GetMd5Hash(bodyDataJson);
                queryString += string.Format("&body_md5={0}", bodyMD5);
            }

            string authToSign = String.Format(requestType + "\n{0}\n{1}", resource, queryString);
            var authSignature = CryptoHelper.GetHmac256(_appSecret, authToSign);

            var requestUrl = resource + "?" + queryString + "&auth_signature=" + authSignature;
            var request = new RestRequest(requestUrl);
            request.RequestFormat = DataFormat.Json;
            request.Method = Method.POST;
            request.AddBody(requestBody);

            return request;
        }

        private void ThrowArgumentExceptionIfNullOrEmpty(string value, string argumentName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(string.Format("{0} cannot be null or empty", argumentName));
            }
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
