using System;
using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private static string appId = Environment.GetEnvironmentVariable("PUSHER_APP_ID");
        private static string appKey = Environment.GetEnvironmentVariable("PUSHER_APP_KEY");
        private static string appSecret = Environment.GetEnvironmentVariable("PUSHER_APP_SECRET");

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
