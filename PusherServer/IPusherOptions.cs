using System;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Interface for Pusher Options
    /// </summary>
    public interface IPusherOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether calls to the Pusher REST API are over HTTP or HTTPS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encrypted; otherwise, <c>false</c>.
        /// </value>
        bool Encrypted { get; set; }

        /// <summary>
        /// Gets or sets the REST API port that the HTTP calls will be made to.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        int Port { get; set; }

        /// <summary>
        /// Gets or sets the Json Serializer
        /// </summary>
        ISerializeObjectsToJson JsonSerializer { get; set; }

        /// <summary>
        /// Gets or sets the Json Deserializer
        /// </summary>
        IDeserializeJsonStrings JsonDeserializer { get; set; }

        /// <summary>
        /// Gets or sets the rest client. Generally only expected to be used for testing.
        /// </summary>
        /// <value>
        /// The rest client.
        /// </value>
        IRestClient RestClient { get; set; }

        /// <summary>
        /// The host of the HTTP API endpoint excluding the scheme e.g. api.pusherapp.com
        /// </summary>
        /// <exception cref="FormatException">If a scheme is found at the start of the host value</exception>
        string HostName { get; set; }

        /// <summary>
        /// The host of the HTTP Api endoind for push notifications e.g. nativepush-cluster1.pusher.com
        /// </summary>
        string HostName_Notificaiton { get; set; }

        /// <summary>
        /// The vrsion of the the push notificaiton service e.g. v1
        /// </summary>
        string Notificaiton_Version { get; set; }

        /// <summary>
        /// The prefix to be used to form the notificaiton service url e.g. server_api
        /// </summary>
        string Notification_Prefix { get; set; }

        /// <summary>
        /// The cluster where the application was created, e.g. eu
        /// </summary>
        string Cluster { get; set; }

        /// <summary>
        /// Gets the base Url based on the set Options
        /// </summary>
        /// <returns>The constructed URL</returns>
        Uri GetBaseUrl();
    }
}
