using System;
using System.Net;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Abstract base class for results coming back from request to the Pusher servers
    /// </summary>
    public abstract class RequestResult : IRequestResult
    {
        string _body = null;
        private HttpStatusCode _statusCode;

        /// <summary>
        /// Constructor to constract the abstract base class for classes derived from RequestResults
        /// </summary>
        /// <param name="response"></param>
        public RequestResult(IRestResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            _body = response.Content;
            _statusCode = response.StatusCode;

            Response = response;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                OriginalContent = response.Content;
            }
        }

        /// <summary>
        /// Gets the Status Code returned in the wrapped Rest Response
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
            protected set { _statusCode = value; }
        }

        /// <summary>
        /// Gets the Body returned in the wrapped Rest Response
        /// </summary>
        public string Body
        {
            get { return _body; }
            protected set { _body = value;  }
        }

        /// <summary>
        /// Gets the original content that was returned in the response, if the response returned was Bad
        /// </summary>
        public string OriginalContent
        {
            get
            {
                return Response != null ? Response.Content : string.Empty;
            }
        }

        /// <summary>
        /// Gets the original response from the rest service
        /// </summary>
        public IRestResponse Response { get; private set; }
    }
}
