using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PusherServer;

namespace PusherRESTDotNet.Authentication
{
    public class PusherAuthenticationHelper
    {
        private string applicationId;
        private string applicationKey;
        private string applicationSecret;
        private IPusher _pusher;

        public PusherAuthenticationHelper(string applicationId, string applicationKey, string applicationSecret)
        {
            this._pusher = new Pusher(applicationId, applicationKey, applicationSecret);
        }

        public string CreateAuthenticatedString(string socketID, string channelName)
        {
            IAuthenticationData signature = _pusher.Authenticate(channelName, socketID);
            string json = JsonConvert.SerializeObject(signature);
            return json;
        }

        public string CreateAuthenticatedString(string socketID, string channelName, PresenceChannelData channelData)
        {
            PusherServer.PresenceChannelData data = new PusherServer.PresenceChannelData()
            {
                user_id = channelData.user_id,
                user_info = channelData.user_info
            };
            IAuthenticationData signature = _pusher.Authenticate(channelName, socketID, data);
            string json = JsonConvert.SerializeObject(signature);
            return json;
        }
    }
}
