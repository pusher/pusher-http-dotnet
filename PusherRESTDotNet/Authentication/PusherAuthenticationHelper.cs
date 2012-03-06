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
            string channelDataString = (channelData == null?"":JsonConvert.SerializeObject(channelData));
            string stringToSign = socketID + ":" + channelName + (string.IsNullOrEmpty(channelDataString)?"":":" + channelDataString);
            string auth = AuthSignatureHelper.GetAuthString(stringToSign, applicationSecret);

            AuthData data = new AuthData();
            data.auth = applicationKey + ":" + auth;
            data.channel_data = channelDataString;

            string json = JsonConvert.SerializeObject(data);
            return json;
        }
    }
}
