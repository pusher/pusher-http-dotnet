namespace PusherServer.RestfulClient
{
    /// <summary>
    /// The contract for a REST request to be made to the Pusher API
    /// </summary>
    public interface IPusherRestRequest
    {
        /// <summary>
        /// Gets or sets the type of RESTful call to make
        /// </summary>
        PusherMethod Method { get; set; }

        /// <summary>
        /// Gets the Resource Uri for this request
        /// </summary>
        string ResourceUri { get; }

        /// <summary>
        /// Gets the content that will be sent with the request
        /// </summary>
        object Content { get; }

        /// <summary>
        /// Adds a body to the request
        /// </summary>
        /// <param name="requestBody">The object to add as the body of the request</param>
        void AddBody(object requestBody);

        /// <summary>
        /// Adds a header to the request
        /// </summary>
        /// <param name="headerName">The name of the header</param>
        /// <param name="value">The value to add to the header</param>
        void AddHeader(string headerName, string value);
    }
}