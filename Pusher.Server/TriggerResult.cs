using System;
using System.Net;
using RestSharp;

namespace Pusher.Server
{
    internal class TriggerResult: ITriggerResult
    {
        string _body = null;
        private HttpStatusCode _statusCode;

        public TriggerResult(IRestResponse response)
        {
            _body = response.Content;
            _statusCode = response.StatusCode;
        }

        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }


        public string Body
        {
            get { return _body; }
        }
    }
}
