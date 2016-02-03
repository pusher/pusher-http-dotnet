using System;
using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private static string DEFAULT_REST_API_HOST = "api.pusherapp.com";
        private static string DEFAULT_WEBSOCKET_API_HOST = "ws.pusherapp.com";

        private static string appId = Environment.GetEnvironmentVariable("PUSHER_APP_ID");
        private static string appKey = Environment.GetEnvironmentVariable("PUSHER_APP_KEY");
        private static string appSecret = Environment.GetEnvironmentVariable("PUSHER_APP_SECRET");
        private static string host = ConfigurationManager.AppSettings.Get("pusher-http-host") ?? DEFAULT_REST_API_HOST;
        private static string wshost = ConfigurationManager.AppSettings.Get("pusher-websocket-host") ?? DEFAULT_WEBSOCKET_API_HOST;

        public static string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
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

        public static string WebSocketHost
        {
            get
            {
                return wshost;
            }
            set
            {
                wshost = value;
            }
        }
    }
}
