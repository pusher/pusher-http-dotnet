using System;
using System.Net;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Abstract base class for results coming back from request to the Pusher servers
    /// </summary>
    public abstract class RequestResult: IRequestResult
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
    }
}
