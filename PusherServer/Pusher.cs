using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PusherServer.RestfulClient;
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

        private readonly IAuthenticatedRequestFactory _factory;

        /// <summary>
        /// Pusher library version information.
        /// </summary>
        public static Version VERSION
        {
            get
            {
                return typeof(Pusher).GetTypeInfo().Assembly.GetName().Version;
            }
        }

        /// <summary>
        /// The Pusher library name.
        /// </summary>
        public static String LIBRARY_NAME
        {
            get
            {
                Attribute attr = typeof(Pusher).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));

                AssemblyProductAttribute adAttr = (AssemblyProductAttribute)attr;
                
                return adAttr.Product;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pusher" /> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appKey">The app key.</param>
        /// <param name="appSecret">The app secret.</param>
        /// <param name="options">(Optional)Additional options to be used with the instance e.g. setting the call to the REST API to be made over HTTPS.</param>
        public Pusher(string appId, string appKey, string appSecret, IPusherOptions options = null)
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

            _factory = new AuthenticatedRequestFactory(appKey, appId, appSecret);
        }

        /// <inheritdoc/>
        public async Task<TriggerResult2> TriggerAsync(string channelName, string eventName, object data, ITriggerOptions options = null)
        {
            return await TriggerAsync(new[] { channelName }, eventName, data, options);
        }

        /// <inheritdoc/>
        public async Task<TriggerResult2> TriggerAsync(string[] channelNames, string eventName, object data, ITriggerOptions options = null)
        {
            if (options == null)
                options = new TriggerOptions();

            var bodyData = CreateTriggerBody(channelNames, eventName, data, options);

            var request = _factory.Build(PusherMethod.POST, "/events", requestBody: bodyData);

            DebugTriggerRequest(request);

            var result = await _options.PusherRestClient.ExecutePostAsync(request);

            DebugTriggerResponse(result);

            return result;
        }

        ///<inheritDoc/>
        public async Task<TriggerResult2> TriggerAsync(Event[] events)
        {
            var bodyData = CreateBatchTriggerBody(events);

            var request = _factory.Build(PusherMethod.POST, "/batch_events", requestBody: bodyData);

            DebugTriggerRequest(request);

            var result = await _options.PusherRestClient.ExecutePostAsync(request);

            DebugTriggerResponse(result);

            return result;
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
            
            var batchEvents = events.Select(e => new BatchEvent
            {
                name = e.EventName,
                channel = e.Channel,
                socket_id = e.SocketId,
                data = _options.JsonSerializer.Serialize(e.Data)
            }).ToArray();

            return new BatchTriggerBody()
            {
                batch = batchEvents
            };
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
            return new AuthenticationData(_appKey, _appSecret, channelName, socketId);
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
                throw new ArgumentNullException(nameof(presenceData));
            }

            return new AuthenticationData(_appKey, _appSecret, channelName, socketId, presenceData);
        }
        
        /// <summary>
        /// Using the provided response, interrogates the Pusher API
        /// </summary>
        /// <typeparam name="T">The type of object to get</typeparam>
        /// <param name="resource">The name of the resource to get</param>
        /// <param name="parameters">(Optional)Any additional parameters required for the Get</param>
        /// <returns>The result of the Get</returns>
        public async Task<IGetResult<T>> GetAsync<T>(string resource, object parameters = null)
        {
            var request = _factory.Build(PusherMethod.GET, resource, parameters);

            var response = await _options.PusherRestClient.ExecuteGetAsync<T>(request);

            return response;
        }

        /// <summary>
        /// Creates a new <see cref="WebHook"/> using the application secret
        /// </summary>
        /// <param name="signature">The signature to use during creation</param>
        /// <param name="body">A JSON string representing the data to use in the Web Hook</param>
        /// <returns>A populated Web Hook</returns>
        public IWebHook ProcessWebHook(string signature, string body)
        {
            return new WebHook(_appSecret, signature, body);
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
        public async Task<IGetResult<T>> FetchUsersFromPresenceChannelAsync<T>(string channelName)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = _factory.Build(PusherMethod.GET, string.Format(ChannelUsersResource, channelName));

            var response = await _options.PusherRestClient.ExecuteGetAsync<T>(request);

            return response;
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
        public async Task<IGetResult<T>> FetchStateForChannelAsync<T>(string channelName, object info = null)
        {
            ThrowArgumentExceptionIfNullOrEmpty(channelName, "channelName");

            var request = _factory.Build(PusherMethod.GET, string.Format(ChannelResource, channelName), info);

            var response = await _options.PusherRestClient.ExecuteGetAsync<T>(request);

            return response;
        }

        /// <inheritDoc/>
        public IGetResult<T> FetchStateForChannels<T>(object info = null)
        {
            var request = CreateAuthenticatedRequest(Method.GET, MultipleChannelsResource, info, null);

            var response = _options.RestClient.Execute(request);

            return new GetResult<T>(response, _options.JsonDeserializer);
        }

        /// <inheritDoc/>
        public async Task<IGetResult<T>> FetchStateForChannelsAsync<T>(object info = null)
        {
            var request = _factory.Build(PusherMethod.GET, MultipleChannelsResource, info);

            var response = await _options.PusherRestClient.ExecuteGetAsync<T>(request);

            return response;
        }

        private void DebugTriggerRequest(IPusherRestRequest request)
        {
            Debug.WriteLine($"Method: {request.Method}{Environment.NewLine}Host: {_options.RestClient.BaseUrl}{Environment.NewLine}Resource: {request.ResourceUri}{Environment.NewLine}Body:{request.Body}");
        }

        private void DebugTriggerResponse(TriggerResult2 response)
        {
            Debug.WriteLine($"Response{Environment.NewLine}StatusCode: {response.StatusCode}{Environment.NewLine}Body: {response.OriginalContent}");
        }

        private IRestRequest CreateAuthenticatedRequest(Method requestType, string resource, object requestParameters, object requestBody)
        {
            SortedDictionary<string, string> queryParams = GetObjectProperties(requestParameters);

            int timeNow = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            queryParams.Add("auth_key", _appKey);
            queryParams.Add("auth_timestamp", timeNow.ToString());
            queryParams.Add("auth_version", "1.0");

            if (requestBody != null)
            {
                JsonSerializer serializer = new JsonSerializer();
                var bodyDataJson = serializer.Serialize(requestBody);
                var bodyMd5 = CryptoHelper.GetMd5Hash(bodyDataJson);
                queryParams.Add("body_md5", bodyMd5);
            }

            string queryString = string.Empty;
            foreach(KeyValuePair<string, string> parameter in queryParams)
            {
                queryString += parameter.Key + "=" + parameter.Value + "&";
            }
            queryString = queryString.TrimEnd('&');

            string path = $"/apps/{_appId}/{resource.TrimStart('/')}";

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

        private static SortedDictionary<string, string> GetObjectProperties(object obj)
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

        private static void ThrowArgumentExceptionIfNullOrEmpty(string value, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{argumentName} cannot be null or empty");
            }
        }
    }
}
