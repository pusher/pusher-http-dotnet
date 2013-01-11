using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Serializers;

namespace Pusher.Server
{
    class AuthenticationSignature: IAuthenticationSignature
    {
        public AuthenticationSignature(string appKey, string appSecret, string channelName, string socketId)
        {
            var stringToSign = socketId + ":" + channelName;
            this.auth = appKey + ":" + CryptoHelper.GetHmac256(appSecret, stringToSign);
        }

        public AuthenticationSignature(string appKey, string appSecret, string channelName, string socketId, PresenceChannelData presenceData)
        {
            var serializer = new JsonSerializer();
            var presenceJson = serializer.Serialize(presenceData);
            var stringToSign = socketId + ":" + channelName + ":" + presenceJson ;
            this.auth = appKey + ":" + CryptoHelper.GetHmac256(appSecret, stringToSign);
        }

        public string auth { get; private set; }
    }
}
