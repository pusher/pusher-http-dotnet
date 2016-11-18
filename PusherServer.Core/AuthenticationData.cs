using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PusherServer
{
    [DataContract]
    class AuthenticationData: IAuthenticationData
    {
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly string _channelName;
        private readonly string _socketId;
        private readonly PresenceChannelData _presenceData;

        public AuthenticationData(string appKey, string appSecret, string channelName, string socketId)
        {
            ValidationHelper.ValidateChannelName(channelName);
            ValidationHelper.ValidateSocketId(socketId);

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
                var stringToSign = _socketId + ":" + _channelName;
                if (_presenceData != null)
                {
                    var presenceJson = JsonConvert.SerializeObject(_presenceData);
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
                    json = JsonConvert.SerializeObject(_presenceData);
                }
                return json;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
