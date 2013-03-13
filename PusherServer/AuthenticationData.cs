using System;
using System.Collections.Generic;
using System.Text;
using RestSharp.Serializers;

namespace PusherServer
{
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

        public string auth
        {
            get
            {
                var serializer = new JsonSerializer();
                var stringToSign = _socketId + ":" + _channelName;
                if (_presenceData != null)
                {
                    var presenceJson = serializer.Serialize(_presenceData);
                    stringToSign += ":" + presenceJson;
                }
                
                return _appKey + ":" + CryptoHelper.GetHmac256(_appSecret, stringToSign);
            }
        }

        /// <summary>
        /// Double encoded JSON containing presence channel user information.
        /// </summary>
        public string channel_data
        {
            get
            {
                var serializer = new JsonSerializer();
                return serializer.Serialize(_presenceData);
            }
        }
    }
}
