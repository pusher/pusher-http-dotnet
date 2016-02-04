using System;
using System.Configuration;

namespace PusherServer.Tests
{
    internal static class Config
    {
        private const string PUSHER_APP_ID = "PUSHER_APP_ID";
        private const string PUSHER_APP_KEY = "PUSHER_APP_KEY";
        private const string PUSHER_APP_SECRET = "PUSHER_APP_SECRET";
        private const string PUSHER_APP_WEB_SOCKET_HOST = "PUSHER_APP_WEB_SOCKET_HOST";
        private const string PUSHER_APP_HOST = "PUSHER_APP_HOST";

        private static string _appId;
        private static string _appKey;
        private static string _appSecret;
        private static string _wsHost;
        private static string _host;

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

            _wsHost = Environment.GetEnvironmentVariable(PUSHER_APP_WEB_SOCKET_HOST);
            if (string.IsNullOrEmpty(_wsHost))
            {
                _wsHost = ConfigurationManager.AppSettings.Get(PUSHER_APP_WEB_SOCKET_HOST);
            }

            _host = Environment.GetEnvironmentVariable(PUSHER_APP_HOST);
            if (string.IsNullOrEmpty(_host))
            {
                _host = ConfigurationManager.AppSettings.Get(PUSHER_APP_HOST);
            }
        }

        public static string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
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

        public static string WebSocketHost
        {
            get
            {
                return _wsHost;
            }
            set
            {
                _wsHost = value;
            }
        }
    }
}
