using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using PusherServer.Exceptions;
using PusherServer.Util;

namespace PusherServer
{
    /// <summary>
    /// The response from a Trigger REST request
    /// </summary>
    public class TriggerResult2 : RequestResult2, ITriggerResult
    {
        /// <summary>
        /// Constructs a new instance of a TriggerResult based upon a passed in Rest Response
        /// </summary>
        /// <param name="response">The Rest Response which will form the basis of this Trigger Result</param>
        /// <param name="responseContent">The response content as a string</param>
        public TriggerResult2(HttpResponseMessage response, string responseContent) : base(response, responseContent)
        {
            EventIdData eventIdData = null;

            try
            {
                eventIdData = JsonConvert.DeserializeObject<EventIdData>(responseContent);
            }
            catch (Exception)
            {
                string msg = $"The response body from the Pusher HTTP endpoint could not be parsed as JSON: {Environment.NewLine}{response.Content}";
                throw new TriggerResponseException(msg);
            }

            EventIds = new ReadOnlyDictionary<string, string>(eventIdData.event_ids);
        }

        /// <inheritDoc/>
        public IDictionary<string, string> EventIds { get; }
    }
}