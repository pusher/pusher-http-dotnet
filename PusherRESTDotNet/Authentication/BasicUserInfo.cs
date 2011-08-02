using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PusherRESTDotNet.Authentication
{
    [DataContract]
    public class BasicUserInfo
    {
        [DataMember]
        public string name { get; set; }
    }
}