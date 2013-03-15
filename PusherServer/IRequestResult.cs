using System;
using System.Net;
namespace PusherServer
{
    public interface IRequestResult
    {
        string Body { get; }
        HttpStatusCode StatusCode { get; }
    }
}
