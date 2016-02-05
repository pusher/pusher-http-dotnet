using System;
using System.Collections.Generic;
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
        private IBodySerializer _serializer;

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
        public Pusher(string appId, string appKey, string appSecret):
            this(appId, appKey, appSecret, null)

        {}

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

            BodySerializer = new DefaultJsonBodySerializer();
        }

        /// <summary>
        /// The serializer to use for the body of the messages.
        /// </summary>
        public IBodySerializer BodySerializer
        {
            get { return _serializer; }
            set { _serializer = value ?? new DefaultJsonBodySerializer(); }
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
            ValidationHelper.ValidateChannelNames(channelNames);
            ValidationHelper.ValidateSocketId(options.SocketId);

            TriggerBody bodyData = new TriggerBody()
            {
                name = eventName,
                data = BodySerializer.Serialize(data),
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
            return new GetResult<T>(response);
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
        public IGetResult<T> FetchUsersFromPrecenceChannel<T>(string channelName)
        {
            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelUsersResource, channelName), null, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response);
        }

        /// <inheritDoc/>
        public void FetchUsersFromPrecenceChannelAsync<T>(string channelName, Action<IGetResult<T>> callback)
        {
            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelUsersResource, channelName), null, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response));
            });
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchStateForChannel<T>(string channelName, object info)
        {
            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelResource, channelName), info, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelAsync<T>(string channelName, object info, Action<IGetResult<T>> callback)
        {
            var request = CreateAuthenticatedRequest(Method.GET, string.Format(ChannelResource, channelName), info, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response));
            });
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchStateForChannels<T>(object info)
        {
            var request = CreateAuthenticatedRequest(Method.GET, MultipleChannelsResource, info, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response);
        }

        /// <inheritDoc/>
        public void FetchStateForChannelsAsync<T>(object info, Action<IGetResult<T>> callback)
        {
            var request = CreateAuthenticatedRequest(Method.GET, MultipleChannelsResource, info, null);

            _options.RestClient.ExecuteAsync(request, response =>
            {
                callback(new GetResult<T>(response));
            });
        }

        #endregion

        private IRestResponse ExecuteTrigger(string[] channelNames, string eventName, object requestBody)
        {
            var request = CreateAuthenticatedRequest(Method.POST, "/events", null, requestBody);

            IRestResponse response = _options.RestClient.Execute(request);
            return response;
        }

        private IRestRequest CreateAuthenticatedRequest(Method requestType, string resource, object requestParameters, object requestBody)
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
            string path = string.Format("/apps/{0}/{1}", this._appId, resource);

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
