using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PusherRESTDotNet.Authentication
{
    [DataContract]
    internal class AuthData
    {
        [DataMember]
        public string auth { get; set; }

        [DataMember]
        public string channel_data { get; set; }
    }
}