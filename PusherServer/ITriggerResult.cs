using System.Collections.Generic;

namespace PusherServer
{
    /// <summary>
    /// Interface for Trigger Request Results
    /// </summary>
    public interface ITriggerResult: IRequestResult
    {
        /// <summary>
        /// Gets the events returned on the Trigger Result
        /// </summary>
        IDictionary<string, string> EventIds { get; }
    }
}
