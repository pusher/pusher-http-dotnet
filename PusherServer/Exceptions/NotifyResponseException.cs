using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer.Exceptions
{
    /// <summary>
    /// Thrown when problems are detected with the response from the Pusher Notify HTTP endpoint.
    /// </summary>
    public class NotifyResponseException : Exception
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="message">Description of the exception</param>
        public NotifyResponseException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="message">Description of the exception</param>
        /// <param name="innerException">The inner exception</param>
        public NotifyResponseException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
