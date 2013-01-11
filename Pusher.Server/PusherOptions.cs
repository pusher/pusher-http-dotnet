using RestSharp;

namespace PusherServer
{
    public class PusherOptions: IPusherOptions
    {
        public IRestClient RestClient { get; set; }
    }
}
