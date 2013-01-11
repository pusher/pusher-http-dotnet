using RestSharp;

namespace Pusher.Server
{
    public interface IPusherOptions
    {
        IRestClient RestClient { get; set; }
    }
}
