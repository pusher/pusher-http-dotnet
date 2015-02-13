using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    public class WebHookData
    {
        private DateTime _time;

        public string time_ms
        {
            get
            {
                // This should not be used.
                return GetUnixTimestampMillis(this.Time).ToString();
            }
            set
            {
                long unixTimeStamp = long.Parse(value);
                _time = DateTimeFromUnixTimestampMillis(unixTimeStamp);
            }
        }
        public Dictionary<string, string>[] events { get; set; }
        public DateTime Time
        {
            get
            {
                return this._time;
            }
        }

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long GetUnixTimestampMillis(DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalMilliseconds;
        }

        private static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }
    }
}
