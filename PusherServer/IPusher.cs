using System;

namespace PusherServer
{
    /// <summary>
    /// Provides access to functionality within the Pusher service such as Trigger to trigger events
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
        /// <returns>The result of the call to the REST API</returns>
        /// <exception cref="Exceptions.TriggerResponseException">
        /// For any non-200 response from the HTTP API or if the response body cannot be parsed as JSON
        /// </exception>
        ITriggerResult Trigger(string channelName, string eventName, object data);

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelNames">The names of the channels the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <returns>The result of the call to the REST API</returns>
        /// <exception cref="Exceptions.TriggerResponseException">
        /// For any non-200 response from the HTTP API or if the response body cannot be parsed as JSON
        /// </exception>
        ITriggerResult Trigger(string[] channelNames, string eventName, object data);

        /// <summary>
        /// Attempting to notify
        /// </summary>
        /// <param name="interests"></param>
        /// <param name="alertText"></param>
        /// <param name="webhook_url"></param>
        /// <param name="webhook_level"></param>
        /// <returns></returns>
        ITriggerResult Notify(string[] interests, string alertText, string webhook_url, string webhook_level)


        /// <summary>
        /// Triggers an event on the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions"/>.</param>
        /// <returns>The result of the call to the REST API</returns>
        /// <exception cref="Exceptions.TriggerResponseException">
        /// For any non-200 response from the HTTP API or if the response body cannot be parsed as JSON
        /// </exception>
        ITriggerResult Trigger(string channelName, string eventName, object data, ITriggerOptions options);

        /// <summary>
        /// Triggers an event on the specified channels.
        /// </summary>
        /// <param name="channelNames">The name of the channels the event should be triggered on.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions"/>.</param>
        /// <returns>The result of the call to the REST API</returns>
        /// <exception cref="Exceptions.TriggerResponseException">
        /// For any non-200 response from the HTTP API or if the response body cannot be parsed as JSON
        /// </exception>
        ITriggerResult Trigger(string[] channelNames, string eventName, object data, ITriggerOptions options);

        /// <summary>
        /// Triggers the events in the passed in array
        /// </summary>
        /// <param name="events">The events to trigger</param>
        /// <returns>The result of the call to the REST API</returns>
        ITriggerResult Trigger(Event[] events);

        /// <summary>
        /// Triggers an event on the specified channels in the background.
        /// </summary>
        /// <param name="channelName">The name of the channel to trigger the event on</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="callback">Method to call when the request has returned.</param>
        void TriggerAsync(string channelName, string eventName, object data, Action<ITriggerResult> callback);

        /// <summary>
        /// Triggers an event on the specified channels in the background.
        /// </summary>
        /// <param name="channelName">The name of the channel to trigger the event on</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions" />.</param>
        /// <param name="callback">Method to call when the request has returned.</param>
        void TriggerAsync(string channelName, string eventName, object data, ITriggerOptions options, Action<ITriggerResult> callback);

        /// <summary>
        /// Triggers an event on the specified channels in the background.
        /// </summary>
        /// <param name="channelNames">The channels to trigger the event on</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="callback">Method to call when the request has returned.</param>
        void TriggerAsync(string[] channelNames, string eventName, object data, Action<ITriggerResult> callback);

        /// <summary>
        /// Triggers an event on the specified channels in the background.
        /// </summary>
        /// <param name="channelNames">The channels to trigger the event on</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be sent with the event. The event payload.</param>
        /// <param name="options">Additional options to be used when triggering the event. See <see cref="ITriggerOptions" />.</param>
        /// <param name="callback">Method to call when the request has returned.</param>
        void TriggerAsync(string[] channelNames, string eventName, object data, ITriggerOptions options, Action<ITriggerResult> callback);

        /// <summary>
        /// Triggers the events in the passed in array asynchronously
        /// </summary>
        /// <param name="events">The events to trigger</param>
        /// <param name="callback">Method to call when the request has returned.</param>
        /// <returns>The result of the call to the REST API</returns>
        void TriggerAsync(Event[] events, Action<ITriggerResult> callback);

        #endregion

        #region Authentication

        /// <summary>
        /// Authenticates the subscription request for a private channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <returns>Authentication data where the required authentication token can be accessed via <see cref="IAuthenticationData.auth"/></returns>
        IAuthenticationData Authenticate(string channelName, string socketId);

        /// <summary>
        /// Authenticates the subscription request for a presence channel.
        /// </summary>
        /// <param name="channelName">Name of the channel to be authenticated.</param>
        /// <param name="socketId">The socket id which uniquely identifies the connection attempting to subscribe to the channel.</param>
        /// <param name="data">Information about the user subscribing to the presence channel.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="data"/> is null</exception>
        /// <returns>Authentication data where the required authentication token can be accessed via <see cref="IAuthenticationData.auth"/></returns>
        IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData data);

        #endregion

        /// <summary>
        /// Makes a GET request to the specified resource. Authentication is handled as part of the call. The data returned from the request is deserizlized to the object type defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">The resource.</param>
        /// <returns>The result of the GET request</returns>
        IGetResult<T> Get<T>(string resource);

        /// <summary>
        /// Makes a GET request to the specified resource. Authentication is handled as part of the call. The data returned from the request is deserizlized to the object type defined by <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">The resource.</param>
        /// <param name="parameters">Additional parameters to be sent as part of the request query string.</param>
        /// <returns>
        /// The result of the GET request
        /// </returns>
        IGetResult<T> Get<T>(string resource, object parameters);

        /// <summary>
        /// Handle an incoming WebHook and validate it.
        /// </summary>
        /// <param name="signature">The signature of the incoming WebHook</param>
        /// <param name="body">The body of the incoming Webhook request</param>
        /// <returns>A WebHook helper.</returns>
        IWebHook ProcessWebHook(string signature, string body);

        /// <summary>
        /// Queries the Pusher API for the Users of a Presence Channel
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="channelName">The name of the channel to query</param>
        /// <returns>The result of the Presence Channel Users query</returns>
        IGetResult<T> FetchUsersFromPresenceChannel<T>(string channelName);

        /// <summary>
        /// Queries the Pusher API for the Users of a Presence Channel asynchronously
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="channelName">The name of the channel to query</param>
        /// <param name="callback">The callback to receive the result of the query</param>
        void FetchUsersFromPresenceChannelAsync<T>(string channelName, Action<IGetResult<T>> callback);

        /// <summary>
        /// Queries the Pusher API for the state of a Channel
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="channelName">The name of the channel to query</param>
        /// <param name="info">An object containing a list of attributes to include in the query</param>
        /// <returns>The result of the Channel State query</returns>
        IGetResult<T> FetchStateForChannel<T>(string channelName, object info);

        /// <summary>
        /// Queries the Pusher API for the state of a Channel asynchronously
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="channelName">The name of the channel to query</param>
        /// <param name="callback">The callback to receive the result of the query</param>
        void FetchStateForChannelAsync<T>(string channelName, Action<IGetResult<T>> callback);

        /// <summary>
        /// Queries the Pusher API for the state of a Channel asynchronously
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="channelName">The name of the channel to query</param>
        /// <param name="info">An object containing a list of attributes to include in the query</param>
        /// <param name="callback">The callback to receive the result of the query</param>
        void FetchStateForChannelAsync<T>(string channelName, object info, Action<IGetResult<T>> callback);

        /// <summary>
        /// Queries the Pusher API for the state of all channels based upon the info object
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="info">An object containing a list of attributes to include in the query</param>
        /// <returns>The result of the Channels State query</returns>
        IGetResult<T> FetchStateForChannels<T>(object info);

        /// <summary>
        /// Queries the Pusher API for the state of all channels based upon the info object
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="callback">The callback to receive the result of the query</param>
        void FetchStateForChannelsAsync<T>(Action<IGetResult<T>> callback);

        /// <summary>
        /// Queries the Pusher API for the state of all channels based upon the info object
        /// </summary>
        /// <typeparam name="T">The type of object that will be returned by the API</typeparam>
        /// <param name="info">An object containing a list of attributes to include in the query</param>
        /// <param name="callback">The callback to receive the result of the query</param>
        void FetchStateForChannelsAsync<T>(object info, Action<IGetResult<T>> callback);
    }
}