using System;
using System.Collections.Generic;
using System.Text;
using RestSharp.Serializers;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PusherServer
{
    [DataContract]
    class AuthenticationData: IAuthenticationData
    {
        private static Regex SOCKET_ID_REGEX = new Regex(@"\A\d+\.\d+\z", RegexOptions.Singleline);

        private string _appKey;
        private string _appSecret;
        private string _channelName;
        private string _socketId;
        private PresenceChannelData _presenceData;

        public AuthenticationData(string appKey, string appSecret, string channelName, string socketId)
        {
            if(SOCKET_ID_REGEX.IsMatch(socketId) == false)
            {
                string msg = 
                    string.Format("socket_id \"{0}\" was not in the form: {1}", socketId, SOCKET_ID_REGEX.ToString());
                throw new FormatException(msg);
            }

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
        [DataMember(Name = "channel_data", IsRequired = false, EmitDefaultValue = false)]
        public string channel_data
        {
            get
            {
                string json = null;
                if (_presenceData != null)
                {
                    var serializer = new JsonSerializer();
                    json = serializer.Serialize(_presenceData);
                }
                return json;
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
