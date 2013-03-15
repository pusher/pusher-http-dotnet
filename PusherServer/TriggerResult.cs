using System;
using System.Net;
using RestSharp;

namespace PusherServer
{
    internal class TriggerResult: RequestResult, ITriggerResult
    {
        public TriggerResult(IRestResponse response):
            base(response)
        {
        }
    }
}
