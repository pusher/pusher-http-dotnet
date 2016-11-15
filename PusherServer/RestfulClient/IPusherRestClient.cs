using System.Threading.Tasks;

namespace PusherServer.RestfulClient
{
    /// <summary>
    /// Contract for a client for the Pusher REST requests
    /// </summary>
    public interface IPusherRestClient
    {
        /// <summary>
        /// Execute a REST request to the Pusher API asynchronously
        /// </summary>
        /// <param name="pusherRestRequest">The request to execute</param>
        /// <returns>The response received from Pusher</returns>
        Task<GetResult2<T>> ExecuteAsync<T>(IPusherRestRequest pusherRestRequest);
    }
}