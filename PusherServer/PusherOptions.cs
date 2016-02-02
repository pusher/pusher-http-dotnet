using System;
using System.Text.RegularExpressions;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Options to be set on the <see cref="Pusher">Pusher</see> instance.
    /// </summary>
    public class PusherOptions: IPusherOptions
    {
        /// <summary>
        /// The default Rest API Host for contacting the Pusher server, it does not contain a cluster name
        /// </summary>
        public const string DEFAULT_REST_API_HOST = "api.pusherapp.com";

        private static int DEFAULT_HTTPS_PORT = 443;
        private static int DEFAULT_HTTP_PORT = 80;
        private static string DEFAULT_HTTP_HOST_NAME = "api.pusherapp.com";

        IRestClient _client;
        bool _encrypted = false;
        bool _portModified = false;
        int _port = DEFAULT_HTTP_PORT;
        string _hostName = null;

        /// <summary>
        /// Gets or sets a value indicating whether calls to the Pusher REST API are over HTTP or HTTPS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encrypted; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the REST API port that the HTTP calls will be made to.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
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
        /// Gets or sets the rest client. Generally only expected to be used for testing.
        /// </summary>
        /// <value>
        /// The rest client.
        /// </value>
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
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("RestClient cannot be null");
                }

                _client = value;
            }
        }

        /// <inheritdoc/>
        public string HostName
        {
            get { return _hostName ?? DEFAULT_REST_API_HOST; }
            set
            {
                if(Regex.IsMatch(value, "^.*://"))
                {
                    string msg = string.Format("The scheme should not be present in the host value: {0}", value);
                    throw new FormatException(msg);
                }

                _hostName = value;
            }
        }

        /// <summary>
        /// Gets the base Url based on the set Options
        /// </summary>
        /// <returns></returns>
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
