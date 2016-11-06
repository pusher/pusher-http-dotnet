using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    /// <summary>
    /// Response payload from notify call
    /// </summary>
    public class NotifySubscriberData
    {
        /// <summary>
        /// Number of subscribers to the listed events
        /// </summary>
        public int number_of_subscribers { get; set; }
    }
}
