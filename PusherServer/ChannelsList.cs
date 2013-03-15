using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PusherServer
{
    /*
    {"channels":{"test_channel":{}}}
    */
    
    [DataContract]
    public class ChannelsList
    {
        private Dictionary<string, Dictionary<string, string>> _channelsInfo = null;

        public Dictionary<string, string> this[string channelName]
        {
            get
            {
                return _channelsInfo[channelName];
            }
            set
            {
                _channelsInfo[channelName] = value;
            }
        }

        [DataMember(Name = "channels")]
        public Dictionary<string, Dictionary<string, string>> Channels
        {
            get
            {
                return _channelsInfo;
            }
            set
            {
                if (_channelsInfo != null)
                {
                    throw new InvalidOperationException("Channels should only be set as part of deserialization.");
                }
                _channelsInfo = value;
            }
        }
    }
}
