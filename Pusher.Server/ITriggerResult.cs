using System.Net;

namespace Pusher.Server
{
    public interface ITriggerResult
    {
        HttpStatusCode StatusCode { get; }

        string Body { get; }
    }
}
