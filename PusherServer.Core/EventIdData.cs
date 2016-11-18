
using System.Collections.Generic;

namespace PusherServer
{
    /// <summary>
    /// Class used for handling the deserialisation of the Trigger HTTP response.
    /// </summary>
    public class EventIdData
    {
        private Dictionary<string, string> _eventIds = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary of channel name to event ID for the triggered event.
        /// </summary>
        public Dictionary<string, string> event_ids
        {
            get
            {
                return _eventIds;
            }
            set
            {
                _eventIds = value;
            }
        }
    }
}