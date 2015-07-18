using System.Collections.Generic;

namespace PusherServer
{
    public interface ITriggerResult: IRequestResult
    {
        IDictionary<string, string> EventIds { get; }
    }
}
