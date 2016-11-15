using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using PusherServer.RestfulClient;
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

        private static int DEFAULT_HTTPS_PORT = 443;
        private static int DEFAULT_HTTP_PORT = 80;

        IRestClient _client;
        IPusherRestClient _pusherClient;
        bool _encrypted = false;
        bool _portModified = false;
        bool _hostSet = false;
        int _port = DEFAULT_HTTP_PORT;
        string _hostName = null;
        string _cluster = null;
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

        public IPusherRestClient PusherRestClient
        {
            get
            {
                if (_pusherClient == null)
                {
                    _pusherClient = new PusherRestClient(GetBaseUrl(), Pusher.LIBRARY_NAME, Pusher.VERSION);
                }

                return _pusherClient;
            }
            set { _pusherClient = value; }
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
            string baseUrl = (Encrypted ? "https" : "http") + "://" + HostName + GetPort();

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
