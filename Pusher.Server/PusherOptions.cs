using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace Pusher.Server
{
    public class PusherOptions: IPusherOptions
    {
        public IRestClient RestClient { get; set; }
    }
}
