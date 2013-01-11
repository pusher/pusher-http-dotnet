using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pusher.Server
{
    /// <summary>
    /// Provides access to functionality within the Pusher service such as <see cref="Trigger"/> to trigger events
    /// and authenticating subscription requests to private and presence channels.
    /// </summary>
    public interface IPusher
    {
        #region Trigger

        /// <summary>
        /// Triggers an event on the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <returns></returns>
        ITriggerResult Trigger(string channelName, string eventName, object data);

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelNames">The names of the channels the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <returns></returns>
        ITriggerResult Trigger(string[] channelNames, string eventName, object data);

        /// <summary>
        /// Triggers an event on the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions"/>.</param>
        /// <returns></returns>
        ITriggerResult Trigger(string channelName, string eventName, object data, ITriggerOptions options);

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelName">The name of the channels the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions"/>.</param>
        /// <returns></returns>
        ITriggerResult Trigger(string[] channelNames, string eventName, object data, ITriggerOptions options);

        //void TriggerAsync(string channelName, string eventName object data, Action<IRestResponse, RestRequestAsyncHandle> callback);

        //void TriggerAsync(string channelName, string eventName object data, ITriggerOptions options, Action<IRestResponse, RestRequestAsyncHandle> callback);

        //void TriggerAsync(string[] channelNames, string eventName object data, Action<IRestResponse, RestRequestAsyncHandle> callback);

        //void TriggerAsync(string[] channelNames, string eventName object data, ITriggerOptions options, Action<IRestResponse, RestRequestAsyncHandle> callback);

        #endregion

        #region Authentication

        /// <summary>
        /// Authenticates the subscription request for a private channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <returns></returns>
        IAuthenticationSignature Authenticate(string channelName, string socketId);

        /// <summary>
        /// Authenticates the subscription request for a presence channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <param name="data">Information about the user subscribing to the presence channel.</param>
        /// <returns></returns>
        IAuthenticationSignature Authenticate(string channelName, string socketId, PresenceChannelData data);

        #endregion
    }

}