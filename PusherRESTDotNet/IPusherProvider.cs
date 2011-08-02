using PusherRESTDotNet.Authentication;
namespace PusherRESTDotNet
{
	public interface IPusherProvider
	{
		/// <summary>
		/// Sends the request. Will throw a WebException if anything other than a 202 response is encountered
		/// </summary>
		/// <param name="request"></param>
		void Trigger(PusherRequest request);

		/// <summary>
		/// Creates and authenticated string for the given channel name and socket ID
		/// </summary>
		/// <param name="channelName">The channel to be authenticated</param>
		/// <param name="socketId">The socket ID to be authenticated</param>
		/// <returns>An authenticated string</returns>
		string Authenticate(string channelName, string socketId);

        /// <summary>
        /// Creates and authenticated string for the given channel name, socket ID and channel data
        /// </summary>
        /// <param name="channelName">The channel to be authenticated</param>
        /// <param name="socketId">The socket ID to be authenticated</param>
        /// <param name="presenceChannelData">The presence channel data</param>
        /// <returns>An authenticated string</returns>
        string Authenticate(string channelName, string socketId, PresenceChannelData presenceChannelData);
    }
}