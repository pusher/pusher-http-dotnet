using RestSharp;

namespace PusherServer
{
    public interface IPusherOptions
    {
        /// <summary>
        /// Gets or sets the rest client. Generally only expected to be used for testing.
        /// </summary>
        /// <value>
        /// The rest client.
        /// </value>
        IRestClient RestClient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether calls to the Pusher REST API are over HTTP or HTTPS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encrypted; otherwise, <c>false</c>.
        /// </value>
        bool Encrypted
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the REST API port that the HTTP calls will be made to.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the REST API host that the HTTP calls will be made to.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        string Host
        {
            get;
            set;
        }
    }
}
