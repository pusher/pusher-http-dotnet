using System;
using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private const string PUSHER_APP_ID = "PUSHER_APP_ID";
        private const string PUSHER_APP_KEY = "PUSHER_APP_KEY";
        private const string PUSHER_APP_SECRET = "PUSHER_APP_SECRET";

        private static string appId;
        private static string appKey;
        private static string appSecret;

        static Config()
        {
            appId = Environment.GetEnvironmentVariable(PUSHER_APP_ID);
            if (string.IsNullOrEmpty(appId))
            {
                appId = ConfigurationManager.AppSettings.Get(PUSHER_APP_ID);
            }

            appKey = Environment.GetEnvironmentVariable(PUSHER_APP_KEY);
            if (string.IsNullOrEmpty(appKey))
            {
                appKey = ConfigurationManager.AppSettings.Get(PUSHER_APP_KEY);
            }

            appSecret = Environment.GetEnvironmentVariable(PUSHER_APP_SECRET);
            if (string.IsNullOrEmpty(appSecret))
            {
                appSecret = ConfigurationManager.AppSettings.Get(PUSHER_APP_SECRET);
            }
        }

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
