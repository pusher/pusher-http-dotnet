using System.Collections.Generic;
namespace PusherServer
{
    /// <summary>
    /// Interface for Trigger Request Results
    /// </summary>
    public interface ITriggerResult: IRequestResult
    {
        IDictionary<string, string> EventIds { get; }
    }
}
