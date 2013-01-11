using System.Net;

namespace PusherServer
{
    public interface ITriggerResult
    {
        HttpStatusCode StatusCode { get; }

        string Body { get; }
    }
}
