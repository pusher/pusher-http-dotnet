using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using RestSharp;
using RestSharp.Serializers;

namespace PusherServer
{
    /// <summary>
    /// Provides access to functionality within the Pusher service such as Trigger to trigger events
    /// and authenticating subscription requests to private and presence channels.
    /// </summary>
    public class Pusher : IPusher
    {
        private const string ChannelUsersResource = "/channels/{0}/users";
        private const string ChannelResource = "/channels/{0}";
        private const string MultipleChannelsResource = "/channels";

        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly IPusherOptions _options;

        /// <summary>
        /// Pusher library version information.
        /// </summary>
        public static Version VERSION
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// The Pusher library name.
        /// </summary>
        public static String LIBRARY_NAME
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                AssemblyProductAttribute adAttr =
                    (AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
                return adAttr.Product;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pusher" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        public Pusher(string appId, string appKey, string appSecret) : this(appId, appKey, appSecret, null)
        { }

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

        #region Trigger
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

            var bodyData = CreateTriggerBody(channelNames, eventName, data, options);
            IRestResponse response = ExecuteTrigger("/events", bodyData);
            TriggerResult result = new TriggerResult(response);
            return result;
        }

        /// <inheritDoc/>
        public ITriggerResult Trigger(Event[] events)
        {
            var bodyData = CreateBatchTriggerBody(events);
            IRestResponse response = ExecuteTrigger("/batch_events", bodyData);
            TriggerResult result = new TriggerResult(response);
            return result;
        }

        /// <inheritdoc/>
        public void TriggerAsync(string channelName, string eventName, object data, Action<ITriggerResult> callback)
        {
            TriggerAsync(channelName, eventName, data, new TriggerOptions(), callback);
        }

        /// <inheritdoc/>
        public void TriggerAsync(string channelName, string eventName, object data, ITriggerOptions options, Action<ITriggerResult> callback)
        {
            TriggerAsync(new string[] { channelName }, eventName, data, options, callback);
        }

        /// <inheritdoc/>
        public void TriggerAsync(string[] channelNames, string eventName, object data, Action<ITriggerResult> callback)
        {
            TriggerAsync(channelNames, eventName, data, new TriggerOptions(), callback);
        }

        /// <inheritdoc/>
        public void TriggerAsync(string[] channelNames, string eventName, object data, ITriggerOptions options, Action<ITriggerResult> callback)
        {
            var bodyData = CreateTriggerBody(channelNames, eventName, data, options);

            ExecuteTriggerAsync("/events", bodyData, baseResponse =>
            {
                if (callback != null)
                {
                    callback(new TriggerResult(baseResponse));
                }
            });
        }

        ///<inheritDoc/>
        public void TriggerAsync(Event[] events, Action<ITriggerResult> callback)
        {
            var bodyData = CreateBatchTriggerBody(events);

            ExecuteTriggerAsync("/batch_events", bodyData, baseResponse =>
            {
                if (callback != null)
                {
                    callback(new TriggerResult(baseResponse));
                }
            });
        }

        /// <inheritDoc/>
        public INotifyResult Notify(string[] interests, string alertText, string alertTitle, string alertSubtitle, string webhook_url, WebhookLevel webhook_level)
        {
            NotifyBody nb = new NotifyBody();
            nb.interests = interests;
            nb.apns.aps.alert.body = alertText;
            nb.apns.aps.alert.title = alertTitle;
            nb.apns.aps.alert.subtitle = alertSubtitle;
            nb.webhook_url = webhook_url;
            nb.webhook_level = webhook_level.ToString();

            IRestResponse response = ExecuteNotify("/notifications", nb);
            NotifyResult result = new NotifyResult(response);
            return result;
        }

        /// <inheritDoc/>
        public INotifyResult Notify(string[] interests, Notification notification, string webhook_url, WebhookLevel webhook_level)
        {
            NotifyBody notifyBody = new NotifyBody();
            notifyBody.interests = interests;
            notifyBody.apns.aps.notification = notification;
            notifyBody.apns.aps.alert.body = "New Message";
            notifyBody.apns.aps.alert.title = notification.title;
            notifyBody.apns.aps.alert.subtitle = notification.subtitle;
            notifyBody.webhook_url = webhook_url;
            notifyBody.webhook_level = webhook_level.ToString();

            IRestResponse response = ExecuteNotify("/notifications", notifyBody);
            NotifyResult result = new NotifyResult(response);
            return result;
        }

        /// <inheritDoc/>
        public INotifyResult Notify(string interest, string alertText, string alertTitle, string alertSubtitle, string webhook_url, WebhookLevel webhook_level)
        {
            return Notify(new string[] { interest }, alertText, alertTitle, alertSubtitle, webhook_url, webhook_level);
        }

        /// <inheritDoc/>
        public void NotifyAsync(string[] interests, string alertText, string alertTitle, string alertSubtitle, string webhook_url, WebhookLevel webhook_level, Action<INotifyResult> callback)
        {
            NotifyBody nb = new NotifyBody();
            nb.interests = interests;
            nb.apns.aps.alert.body = alertText;
            nb.webhook_url = webhook_url;
            nb.webhook_level = webhook_level.ToString();
            nb.apns.aps.alert.title = alertTitle;
            nb.apns.aps.alert.subtitle = alertSubtitle;

            ExecuteNotifyAsync("/notifications", nb, baseResponse =>
            {
                if (callback != null)
                {
                    callback(new NotifyResult(baseResponse));
                }
            });
        }

        private TriggerBody CreateTriggerBody(string[] channelNames, string eventName, object data, ITriggerOptions options)
        {
            ValidationHelper.ValidateChannelNames(channelNames);
            ValidationHelper.ValidateSocketId(options.SocketId);

            TriggerBody bodyData = new TriggerBody()
            {
                name = eventName,
                data = _options.JsonSerializer.Serialize(data),
                channels = channelNames
            };

            if (string.IsNullOrEmpty(options.SocketId) == false)
            {
                bodyData.socket_id = options.SocketId;
            }

            return bodyData;
        }

        private BatchTriggerBody CreateBatchTriggerBody(Event[] events)
        {
            ValidationHelper.ValidateBatchEvents(events);

            var batchEvents = Array.ConvertAll(events, e => new BatchEvent
            {
                name = e.EventName,
                channel = e.Channel,
                socket_id = e.SocketId,
                data = _options.JsonSerializer.Serialize(e.Data)
            });

            return new BatchTriggerBody()
            {
                batch = batchEvents
            };
        }

        #endregion

        #region Authentication
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
        /// Authenticates the subscription request for a presence channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <param name="presenceData">Information about the user subscribing to the presence channel.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="presenceData"/> is null</exception>
        /// <returns>Authentication data where the required authentication token can be accessed via <see cref="IAuthenticationData.auth"/></returns>
        public IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData presenceData)
        {
            if(presenceData == null)
            {
                throw new ArgumentNullException("presenceData");
            }
            return new AuthenticationData(this._appKey, this._appSecret, channelName, socketId, presenceData);
        }
        #endregion

        #region Get

        /// <summary>
        /// Using the provided response, interrogates the Pusher API
        /// </summary>
        /// <typeparam name="T">The type of object to get</typeparam>
        /// <param name="resource">The name of the resource to get</param>
        /// <returns>The result of the Get</returns>
        public IGetResult<T> Get<T>(string resource)
        {
            return Get<T>(resource, null);
        }

        /// <summary>
        /// Using the provided response, interrogates the Pusher API
        /// </summary>
        /// <typeparam name="T">The type of object to get</typeparam>
        /// <param name="resource">The name of the resource to get</param>
        /// <param name="parameters">Any additional parameters required for the Get</param>
        /// <returns>The result of the Get</returns>
        public IGetResult<T> Get<T>(string resource, object parameters)
        {
            var request = CreateAuthenticatedRequest(Method.GET, resource, parameters, null);

            IRestResponse response = _options.RestClient.Execute(request);
            return new GetResult<T>(response, _options.JsonDeserializer);
        }

        /// <summary>
        /// Creates a new <see cref="WebHook"/> using the application secret
        /// </summary>
        /// <param name="signature">The signature to use during creation</param>
        /// <param name="body">A JSON string representing the data to use in the Web Hook</param>
        /// <returns>A populated Web Hook</returns>
        public IWebHook ProcessWebHook(string signature, string body)
        {
            return new WebHook(this._appSecret, signature, body);
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchUsersFromPresenceChannel<T>(string channelName)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelUsersResource, channelName), null, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response, _options.JsonDeserializer);
        }

        /// <inheritDoc/>
        public void FetchUsersFromPresenceChannelAsync<T>(string channelName, Action<IGetResult<T>> callback)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelUsersResource, channelName), null, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response, _options.JsonDeserializer));
            });
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchStateForChannel<T>(string channelName, object info = null)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelResource, channelName), info, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response, _options.JsonDeserializer);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelAsync<T>(string channelName, Action<IGetResult<T>> callback)
        {
            FetchStateForChannelAsync<T>(channelName, null, callback);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelAsync<T>(string channelName, object info, Action<IGetResult<T>> callback)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelResource, channelName), info, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response, _options.JsonDeserializer));
            });
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchStateForChannels<T>(object info = null)
        {
            var request = CreateAuthenticatedRequest(Method.GET, MultipleChannelsResource, info, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response, _options.JsonDeserializer);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelsAsync<T>(Action<IGetResult<T>> callback)
        {
            FetchStateForChannelsAsync<T>(null, callback);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelsAsync<T>(object info, Action<IGetResult<T>> callback)
        {
            var request = CreateAuthenticatedRequest(Method.GET, MultipleChannelsResource, info, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response, _options.JsonDeserializer));
            });
        }

        #endregion

        private IRestResponse ExecuteTrigger(string path, object requestBody)
        {
            _options.RestClient.BaseUrl = _options.GetBaseUrl();
            var request = CreateAuthenticatedRequest(Method.POST, path, null, requestBody);
            Debug.WriteLine(string.Format("Method: {1}{0}Host: {2}{0}Resource: {3}{0}Parameters:{4}",
                Environment.NewLine,
                request.Method,
                _options.RestClient.BaseUrl,
                request.Resource,
                string.Join(",", Array.ConvertAll(request.Parameters.ConvertAll(p => p.Name + "=" + p.Value).ToArray(), i => i.ToString()))
            ));

            IRestResponse response = _options.RestClient.Execute(request);

            Debug.WriteLine(string.Format("Response{0}StatusCode: {1}{0}Body: {2}",
                Environment.NewLine,
                response.StatusCode,
                response.Content));

            return response;
        }

        private void ExecuteTriggerAsync(string path, object requestBody, Action<IRestResponse> callback)
        {
            _options.RestClient.BaseUrl = _options.GetBaseUrl();

            var request = CreateAuthenticatedRequest(Method.POST, path, null, requestBody);
            _options.RestClient.ExecuteAsync(request, callback);
        }

        private void ExecuteNotifyAsync(string path, object requestBody, Action<IRestResponse> callback)
        {
            _options.RestClient.BaseUrl = _options.GetBaseUrl();

            var request = CreateAuthenticatedRequest(Method.POST, path, null, requestBody,true);
            _options.RestClient.ExecuteAsync(request, callback);
        }

        private IRestResponse ExecuteNotify(string path, object requestBody)
        {
            _options.RestClient.BaseUrl = _options.GetBaseUrl();
            var request = CreateAuthenticatedRequest(Method.POST, path, null, requestBody, true);
            Debug.WriteLine(string.Format("Method: {1}{0}Host: {2}{0}Resource: {3}{0}Parameters:{4}",
                Environment.NewLine,
                request.Method,
                _options.RestClient.BaseUrl,
                request.Resource,
                string.Join(",", Array.ConvertAll(request.Parameters.ConvertAll(p => p.Name + "=" + p.Value).ToArray(), i => i.ToString()))
            ));

            IRestResponse response = _options.RestClient.Execute(request);

            Debug.WriteLine(string.Format("Response{0}StatusCode: {1}{0}Body: {2}",
                Environment.NewLine,
                response.StatusCode,
                response.Content));

            return response;
        }

        private IRestRequest CreateAuthenticatedRequest(Method requestType, string resource, object requestParameters, object requestBody, bool nativeAPI = false)
        {
            SortedDictionary<string, string> queryParams = GetObjectProperties(requestParameters);

            int timeNow = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            queryParams.Add("auth_key", this._appKey);
            queryParams.Add("auth_timestamp", timeNow.ToString());
            queryParams.Add("auth_version", "1.0");

            if (requestBody != null)
            {
                JsonSerializer serializer = new JsonSerializer();
                var bodyDataJson = serializer.Serialize(requestBody);
                var bodyMD5 = CryptoHelper.GetMd5Hash(bodyDataJson);
                queryParams.Add("body_md5", bodyMD5);
            }

            string queryString = string.Empty;
            foreach(KeyValuePair<string, string> parameter in queryParams)
            {
                queryString += parameter.Key + "=" + parameter.Value + "&";
            }
            queryString = queryString.TrimEnd('&');

            resource = resource.TrimStart('/');
            string path;
            if (!nativeAPI)
            {
                path = string.Format("/apps/{0}/{1}", this._appId, resource);
            }else
            {
                path = string.Format("/{0}/{1}/apps/{2}/{3}", _options.Notification_Prefix,_options.Notificaiton_Version, this._appId, resource);
            }

            string authToSign = String.Format(
                Enum.GetName(requestType.GetType(), requestType) +
                "\n{0}\n{1}",
                path,
                queryString);

            var authSignature = CryptoHelper.GetHmac256(_appSecret, authToSign);

            var requestUrl = path + "?" + queryString + "&auth_signature=" + authSignature;
            var request = new RestRequest(requestUrl);
            request.RequestFormat = DataFormat.Json;
            request.Method = requestType;
            request.AddBody(requestBody);

            request.AddHeader("Pusher-Library-Name", LIBRARY_NAME);
            request.AddHeader("Pusher-Library-Version", VERSION.ToString(3));

            return request;
        }

        private SortedDictionary<string, string> GetObjectProperties(object obj)
        {
            SortedDictionary<string, string> properties = new SortedDictionary<string, string>();

            if (obj != null)
            {
                Type objType = obj.GetType();
                IList<PropertyInfo> propertyInfos = new List<PropertyInfo>(objType.GetProperties());

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    properties.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null).ToString());
                }
            }

            return properties;
        }

        private void ThrowArgumentExceptionIfNullOrEmpty(string value, string argumentName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(string.Format("{0} cannot be null or empty", argumentName));
            }
        }
    }
}
