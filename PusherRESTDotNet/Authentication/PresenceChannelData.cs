using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PusherRESTDotNet.Authentication
{
    [DataContract]
    public class PresenceChannelData
    {
        [DataMember]
        public string user_id { get; set; }

        [DataMember]
        public object user_info { get; set; }
    }
}