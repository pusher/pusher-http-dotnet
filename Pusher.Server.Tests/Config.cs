using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Pusher.Server.Tests
{
    internal static class Config
    {
        public static string AppId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("pusher-app-id");
            }
        }

        public static string AppKey
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("pusher-app-key");
            }
        }

        public static string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("pusher-app-secret");
            }
        }
    }
}
