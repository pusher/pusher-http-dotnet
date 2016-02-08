using System;
using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private const string PUSHER_APP_ID = "PUSHER_APP_ID";
        private const string PUSHER_APP_KEY = "PUSHER_APP_KEY";
        private const string PUSHER_APP_SECRET = "PUSHER_APP_SECRET";
        private const string PUSHER_HTTP_HOST = "PUSHER_APP_HOST";
        private const string PUSHER_WEBSOCKET_HOST = "PUSHER_WEBSOCKET_HOST";

        private static string _appId;
        private static string _appKey;
        private static string _appSecret;
        private static string _httpHost;
        private static string _websocketHost;

        static Config()
        {
            _appId = Environment.GetEnvironmentVariable(PUSHER_APP_ID);
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = ConfigurationManager.AppSettings.Get(PUSHER_APP_ID);
            }

            _appKey = Environment.GetEnvironmentVariable(PUSHER_APP_KEY);
            if (string.IsNullOrEmpty(_appKey))
            {
                _appKey = ConfigurationManager.AppSettings.Get(PUSHER_APP_KEY);
            }

            _appSecret = Environment.GetEnvironmentVariable(PUSHER_APP_SECRET);
            if (string.IsNullOrEmpty(_appSecret))
            {
                _appSecret = ConfigurationManager.AppSettings.Get(PUSHER_APP_SECRET);
            }

            _httpHost = Environment.GetEnvironmentVariable(PUSHER_HTTP_HOST);
            if (string.IsNullOrEmpty(_httpHost))
            {
                _httpHost = ConfigurationManager.AppSettings.Get(PUSHER_HTTP_HOST);
            }

            _websocketHost = Environment.GetEnvironmentVariable(PUSHER_WEBSOCKET_HOST);
            if (string.IsNullOrEmpty(_websocketHost))
            {
                _websocketHost = ConfigurationManager.AppSettings.Get(PUSHER_WEBSOCKET_HOST);
            }
        }

        public static string AppId
        {
            get
            {
                return _appId;
            }
            set
            {
                _appId = value;
            }
        }

        public static string AppKey
        {
            get
            {
                return _appKey;
            }
            set
            {
                _appKey = value;
            }
        }

        public static string AppSecret
        {
            get
            {
                return _appSecret;
            }
            set
            {
                _appSecret = value;
            }
        }

        public static string HttpHost
        {
            get
            {
                return _httpHost;
            }
            set
            {
                _httpHost = value;
            }
        }

        public static string WebSocketHost
        {
            get
            {
                return _websocketHost;
            }
            set
            {
                _websocketHost = value;
            }
        }
    }
}
