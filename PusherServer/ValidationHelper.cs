using System;
using System.Text.RegularExpressions;

namespace PusherServer
{
    internal static class ValidationHelper
    {
        private static Regex SOCKET_ID_REGEX = new Regex(@"\A\d+\.\d+\z", RegexOptions.Singleline);

        internal static void ValidateSocketId(string socketId)
        {
            if (socketId != null && SOCKET_ID_REGEX.IsMatch(socketId) == false)
            {
                string msg =
                    string.Format("socket_id \"{0}\" was not in the form: {1}", socketId, SOCKET_ID_REGEX.ToString());
                throw new FormatException(msg);
            }
        }
    }
}
