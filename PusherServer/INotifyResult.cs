using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PusherServer
{
    /// <summary>
    /// Interface for Notify Request Results
    /// </summary>
    public interface INotifyResult : IRequestResult
    {
        /// <summary>
        /// Number of subscribers
        /// </summary>
        int NumberOfSubscribers { get; }
    }
}
