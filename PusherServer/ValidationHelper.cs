using System;
using System.Text.RegularExpressions;

namespace PusherServer
{
    /// <summary>
    /// Helps validation of channel names and socket_id values.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// A regular expression to check that a channel name is in a format allowed and accepted by Pusher.
        /// </summary>
        public static Regex CHANNEL_NAME_REGEX = new Regex(@"\A[a-zA-Z0-9_=@,.;\-]+\z", RegexOptions.Singleline);

        /// <summary>
        /// The maximum length of a channel name allowed by Pusher.
        /// </summary>
        public static int CHANNEL_NAME_MAX_LENGTH = 164;

        /// <summary>
        /// A regular expression to check that a socket_id is in a format allowed and accepted by Pusher.
        /// </summary>
        public static Regex SOCKET_ID_REGEX = new Regex(@"\A\d+\.\d+\z", RegexOptions.Singleline);

        /// <summary>
        /// The maximum event batch size accepted by Pusher
        /// </summary>
        public static int MAX_BATCH_SIZE = 100;

        /// <summary>
        /// Validate a socket_id value
        /// </summary>
        /// <param name="socketId">The value to be checked.</param>
        /// <exception cref="FormatException">If the socket_id name is not in the allowed format.</exception>
        internal static void ValidateSocketId(string socketId)
        {
            if (socketId != null && SOCKET_ID_REGEX.IsMatch(socketId) == false)
            {
                string msg = $"socket_id \"{socketId}\" was not in the form: {SOCKET_ID_REGEX}";
                throw new FormatException(msg);
            }
        }

        /// <summary>
        /// Validate a single channel name is in the allowed format.
        /// </summary>
        /// <param name="channelName">The channel name to be checked</param>
        /// <exception cref="FormatException">If the channel name is not in the allowed format.</exception>
        internal static void ValidateChannelName(string channelName)
        {
            if(channelName.Length > CHANNEL_NAME_MAX_LENGTH)
            {
                string msg = $"The length of the channel name was greater than the allowed {CHANNEL_NAME_MAX_LENGTH} characters";
                throw new ArgumentOutOfRangeException(nameof(channelName), msg);
            }

            if (CHANNEL_NAME_REGEX.IsMatch(channelName) == false)
            {
                string msg = $"channel name \"{channelName}\" was not in the form: {CHANNEL_NAME_REGEX}";
                throw new FormatException(msg);
            }
        }

        /// <summary>
        /// Validate an array of channel names
        /// </summary>
        /// <param name="channelNames">The array of channel names</param>
        /// <exception cref="FormatException">If any channel names are not in the allowed format.</exception>
        internal static void ValidateChannelNames(string[] channelNames)
        {
            foreach(string name in channelNames)
            {
                ValidateChannelName(name);
            }
        }

        internal static void ValidateBatchEvents(Event[] events)
        {
            if (events.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(events), $"Only {MAX_BATCH_SIZE} events permitted per batch, {events.Length} submitted");
            }

            foreach (Event e in events)
            {
                ValidateChannelName(e.Channel);
                ValidateSocketId(e.SocketId);
            }
        }
    }
}
