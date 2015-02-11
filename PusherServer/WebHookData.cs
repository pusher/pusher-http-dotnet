using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    public class WebHookData
    {
        public string time_ms { get; set; }
        public Dictionary<string, string>[] events { get; set; }
    }
}
