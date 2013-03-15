using System.Net;
using RestSharp;

namespace PusherServer
{
    public abstract class RequestResult: IRequestResult
    {
        string _body = null;
        private HttpStatusCode _statusCode;

        public RequestResult(IRestResponse response)
        {
            _body = response.Content;
            _statusCode = response.StatusCode;
        }

        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
            protected set { _statusCode = value; }
        }

        public string Body
        {
            get { return _body; }
            protected set { _body = value;  }
        }
    }
}
