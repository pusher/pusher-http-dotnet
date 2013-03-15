using PusherClient;
using System.Web.Script.Serialization;

namespace PusherServer.Tests.Helpers
{
    internal class InMemoryAuthorizer: IAuthorizer
    {
        PusherServer.Pusher _pusher;
        PresenceChannelData _presenceData;

        public InMemoryAuthorizer(PusherServer.Pusher pusher):
            this(pusher, null)
        {
        }

        public InMemoryAuthorizer(PusherServer.Pusher pusher, PresenceChannelData presenceData)
        {
            _pusher = pusher;
            _presenceData = presenceData;
        }

        public string Authorize(string channelName, string socketId)
        {
            var auth = _pusher.Authenticate(channelName, socketId, _presenceData);
            return auth.ToJson();
        }
    }
}
