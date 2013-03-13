using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private static string appId = ConfigurationManager.AppSettings.Get("pusher-app-id");
        private static string appKey = ConfigurationManager.AppSettings.Get("pusher-app-key");
        private static string appSecret = ConfigurationManager.AppSettings.Get("pusher-app-secret");

        public static string AppId
        {
            get
            {
                return appId;
            }
            set
            {
                appId = value;
            }
        }

        public static string AppKey
        {
            get
            {
                return appKey;
            }
            set
            {
                appKey = value;
            }
        }

        public static string AppSecret
        {
            get
            {
                return appSecret;
            }
            set
            {
                appSecret = value;
            }
        }
    }
}
