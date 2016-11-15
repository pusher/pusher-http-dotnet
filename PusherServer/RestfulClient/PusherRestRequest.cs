using System;
using System.Collections.Generic;

namespace PusherServer.RestfulClient
{
    /// <summary>
    /// A REST request to be made to the Pusher API
    /// </summary>
    public class PusherRestRequest : IPusherRestRequest
    {
        private readonly string _requestUrl;
        private object _requestBody;
        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// Creates a new REST request to make back to Pusher HQ
        /// </summary>
        /// <param name="requestUrl">The URL to call</param>
        public PusherRestRequest(string requestUrl)
        {
            _headers = new Dictionary<string, string>();
            _requestUrl = requestUrl;
        }

        /// <inheritdoc/>
        public PusherMethod Method { get; set; }

        /// <inheritdoc/>
        public string ResourceUri => _requestUrl;

        /// <inheritdoc/>
        public object Content { get; }

        /// <inheritdoc/>
        public void AddBody(object requestBody)
        {
            _requestBody = requestBody;
        }

        /// <inheritdoc/>
        public void AddHeader(string headerName, string value)
        {
            _headers.Add(headerName, value);
        }
    }
}