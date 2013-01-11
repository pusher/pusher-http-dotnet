using RestSharp;

namespace PusherServer
{
    public interface IPusherOptions
    {
        IRestClient RestClient { get; set; }
    }
}
