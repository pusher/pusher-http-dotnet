using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PusherRESTDotNet.Authentication
{
    public class PusherAuthenticationHelper
    {
        private string applicationId;
        private string applicationKey;
        private string applicationSecret;

        public PusherAuthenticationHelper(string applicationId, string applicationKey, string applicationSecret)
        {
            this.applicationId = applicationId;
            this.applicationKey = applicationKey;
            this.applicationSecret = applicationSecret;
        }

        public string CreateAuthenticatedString(string socketID, string channelName)
        {
            string auth = AuthSignatureHelper.GetAuthString(socketID + ":" + channelName, applicationSecret);

            AuthData data = new AuthData();
            data.auth = applicationKey + ":" + auth;

            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        public string CreateAuthenticatedString(string socketID, string channelName, PresenceChannelData channelData)
        {
            string channel = (channelData == null?"":JsonConvert.SerializeObject(channelData));
            string auth = AuthSignatureHelper.GetAuthString(socketID + ":" + channelName + ":" + channel, applicationSecret);

            AuthData data = new AuthData();
            data.auth = applicationKey + ":" + auth;
            data.channel_data = channel;

            string json = JsonConvert.SerializeObject(data);
            return json;
        }
    }
}
