using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    /// <summary>
    /// Class used for handling the deserialisation of the Trigger HTTP response.
    /// </summary>
    public class EventIdData
    {
        /// <summary>
        /// Dictionary of channel name to event ID for the triggered event.
        /// </summary>
        public Dictionary<string, string> event_ids { get; set; }
    }
}
