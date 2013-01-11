using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherRESTDotNet
{
    public class PusherConfig
    {
        public static readonly string DEFAULT_SCHEME = Uri.UriSchemeHttp;
        public const string DEFAULT_HOST = "api.pusherapp.com";
        public const int DEFAULT_POST = 80;

        private string _scheme = DEFAULT_SCHEME;
        private string _host = DEFAULT_HOST;
        private int _port = DEFAULT_POST;

        /// <summary>
        /// The Scheme of the Uri. Either "http" or "https".
        /// </summary>
        /// <exception cref="ArgumentException">If the Scheme is outside of the allowed values.</exception>
        public string Scheme
        {
            get
            {
                return _scheme;
            }
            set
            {
                if (value != Uri.UriSchemeHttp && value != Uri.UriSchemeHttps)
                {
                    throw new ArgumentException("Scheme must be either \"" + Uri.UriSchemeHttp + "\"" +
                        "or \"" + Uri.UriSchemeHttps + "\"");
                }
                _scheme = value;
            }
        }

        /// <summary>
        /// The host of the Uri.
        /// </summary>
        public string Host
        {
            get
            {
                return this._host;
            }
            set
            {
                this._host = value;
            }
        }

        /// <summary>
        /// The port of the Uri.
        /// </summary>
        public int Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value;
            }
        }
    }
}
