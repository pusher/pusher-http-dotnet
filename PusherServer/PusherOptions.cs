using System;
using System.Text.RegularExpressions;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Options to be set on the <see cref="Pusher">Pusher</see> instance.
    /// </summary>
    public class PusherOptions : IPusherOptions
    {
        /// <summary>
        /// The default Rest API Host for contacting the Pusher server, it does not contain a cluster name
        /// </summary>
        public const string DEFAULT_REST_API_HOST = "api.pusherapp.com";
        /// <summary>
        /// The default Notificaiton API Host for contacting the Noitificaiton server
        /// </summary>
        public const string DEFAULT_NOTIFICATION_REST_API_HOST = "nativepush-cluster1.pusher.com";

        /// <summary>
        /// The default version for the Notificaiton service
        /// </summary>
        public const string DEFAULT_NOTIFICATION_VERSION = "v1";

        /// <summary>
        /// The default prefix for the Notificaiton Service
        /// </summary>
        public const string DEFAULT_NOTIFICATION_PREFIX = "server_api";

        private static int DEFAULT_HTTPS_PORT = 443;
        private static int DEFAULT_HTTP_PORT = 80;

        IRestClient _client;
        bool _encrypted = false;
        bool _portModified = false;
        bool _hostSet = false;
        int _port = DEFAULT_HTTP_PORT;
        string _hostName = null;
        string _notificationHostName = null;
        string _cluster = null;
        string _notificationVersion = null;
        string _notificationPrefix = null;
        ISerializeObjectsToJson _jsonSerializer;
        IDeserializeJsonStrings _jsonDeserializer;

        /// <inheritedDoc/>
        public bool Encrypted
        {
            get
            {
                return _encrypted;
            }
            set
            {
                _encrypted = value;
                if (_encrypted && _portModified == false)
                {
                    _port = DEFAULT_HTTPS_PORT;
                }
            }
        }

        /// <inheritDoc/>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                _portModified = true;
            }
        }

        /// <summary>
        /// Set the cluster only if there is no custom host defined.
        /// </summary>
        public string Cluster
        {
          get
          {
            return _cluster;
          }
          set
          {
            if (_hostSet == false) {
              _cluster = value;
              _hostName = "api-"+_cluster+".pusher.com";
            }
          }
        }

        /// <inheritDoc/>
        public IRestClient RestClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient(GetBaseUrl());
                }

                return _client;
            }
            set { _client = value; }
        }

        /// <summary>
        /// Gets or sets the HostName to use in the base URL
        /// </summary>
        public string HostName
        {
            get
            {
                return _hostName ?? DEFAULT_REST_API_HOST;
            }
            set
            {
                if (Regex.IsMatch(value, "^.*://"))
                {
                    string msg = string.Format("The scheme should not be present in the host value: {0}", value);
                    throw new FormatException(msg);
                }

                _hostSet = true;
                _cluster = null;
                _hostName = value;
            }
        }

        /// <summary>
        /// Gets or sets the HostName to use in the notificaiton base URL
        /// </summary>
        public string HostName_Notificaiton
        {
            get
            {
                return _notificationHostName ?? DEFAULT_NOTIFICATION_REST_API_HOST;
            }
            set
            {
                if (Regex.IsMatch(value, "^.*://"))
                {
                    string msg = string.Format("The scheme should not be present in the host value: {0}", value);
                    throw new FormatException(msg);
                }

                _hostSet = true;
                _cluster = null;
                _notificationHostName = value;
            }
        }
        /// <summary>
        /// The vrsion of the the push notificaiton service e.g. v1
        /// </summary>
        public string Notificaiton_Version
        {
            get
            {
                return _notificationVersion ?? DEFAULT_NOTIFICATION_VERSION;
            }
            set
            {
                _notificationVersion = value;
            }
        }

        /// <summary>
        /// The prefix to be used to form the notificaiton service url e.g. server_api
        /// </summary>
        public string Notification_Prefix
        {
            get
            {
                return _notificationPrefix ?? DEFAULT_NOTIFICATION_PREFIX;
            }
            set
            {

                _notificationPrefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the Json Serializer
        /// </summary>
        public ISerializeObjectsToJson JsonSerializer
        {
            get
            {
                if (_jsonSerializer == null)
                {
                    _jsonSerializer = new DefaultSerializer();
                }

                return _jsonSerializer;

            }
            set { _jsonSerializer = value; }
        }

        /// <summary>
        /// Gets or sets the Json Deserializer
        /// </summary>
        public IDeserializeJsonStrings JsonDeserializer
        {
            get
            {
                if (_jsonDeserializer == null)
                {
                    _jsonDeserializer = new DefaultDeserializer();
                }

                return _jsonDeserializer;
            }
            set
            {
                _jsonDeserializer = value;
            }
        }

        /// <inheritDoc/>
        public Uri GetBaseUrl()
        {
            string baseUrl = (Encrypted ? "https" : "http") + "://" + HostName;// + GetPort();

            return new Uri(baseUrl);
        }

        private string GetPort()
        {
            var port = string.Empty;

            if (Port != DEFAULT_HTTP_PORT)
            {
                port += (":" + Port);
            }

            return port;
        }
    }
}
