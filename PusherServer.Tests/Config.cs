using PusherServer.Tests.Helpers;

namespace PusherServer.Tests
{
    internal static class Config
    {
        static Config()
        {
            IApplicationConfig config = EnvironmentVariableConfigLoader.Default.Load();
            if (string.IsNullOrWhiteSpace(config.AppKey))
            {
                config = JsonFileConfigLoader.Default.Load();
            }

            AppId = config.AppId;
            AppKey = config.AppKey;
            AppSecret = config.AppSecret;
            HttpHost = config.HttpHost;
            WebSocketHost = config.WebSocketHost;
        }

        public static string AppId { get; private set; }

        public static string AppKey { get; private set; }

        public static string AppSecret { get; private set; }

        public static string HttpHost { get; private set; }

        public static string WebSocketHost { get; private set; }
    }
}
