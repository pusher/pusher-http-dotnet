using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    public class NotifyBody
    {
        public string[] interests { get; set; }
        public Apns apns { get; set; }
        public string webhook_url { get; set; }
        public string webhook_level { get; set; }

        public NotifyBody()
        {
            apns = new Apns();
        }
    }

    public class Apns
    {
        public Aps aps { get; set; }

        public Apns()
        {
            aps = new Aps();
        }
    }

    public class Aps
    {
        public Alert alert { get; set; }

        public Aps()
        {
            alert = new Alert();
        }
    }

    public class Alert
    {
        public string body { get; set; }
    }

}
