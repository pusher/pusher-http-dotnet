using System;
using System.Collections.Generic;
using System.Text;
using RestSharp.Serializers;
using System.Runtime.Serialization;

namespace PusherServer
{
    [DataContract]
    class AuthenticationData: IAuthenticationData
    {
        private string _appKey;
        private string _appSecret;
        private string _channelName;
        private string _socketId;
        private PresenceChannelData _presenceData;

        public AuthenticationData(string appKey, string appSecret, string channelName, string socketId)
        {
            _appKey = appKey;
            _appSecret = appSecret;
            _channelName = channelName;
            _socketId = socketId;
        }

        public AuthenticationData(string appKey, string appSecret, string channelName, string socketId, PresenceChannelData presenceData):
            this(appKey, appSecret, channelName, socketId)
        {
            _presenceData = presenceData;
        }

        [DataMember(Name = "auth", IsRequired = true)]
        public string auth
        {
            get
            {
                var serializer = new JsonSerializer();
                var stringToSign = _socketId + ":" + _channelName;
                var presenceJson = serializer.Serialize(_presenceData);
                stringToSign += ":" + presenceJson;
                
                return _appKey + ":" + CryptoHelper.GetHmac256(_appSecret, stringToSign);
            }
        }

        /// <summary>
        /// Double encoded JSON containing presence channel user information.
        /// </summary>
        [DataMember(Name = "channel_data", IsRequired = false)]
        public string channel_data
        {
            get
            {
                var serializer = new JsonSerializer();
                return serializer.Serialize(_presenceData);
            }
        }

        public string ToJson()
        {
            var serializer = new JsonSerializer();
            return serializer.Serialize(this);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
