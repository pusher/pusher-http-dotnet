using System;
using System.Net;
using System.Collections.Generic;
using PusherServer.Exceptions;
using PusherServer.Util;
using RestSharp;
using Newtonsoft.Json;

namespace PusherServer
{
    internal class TriggerResult : RequestResult, ITriggerResult
    {
        private readonly IDictionary<string, string> _eventIds;

        /// <summary>
        /// Constructs a new instance of a TriggerResult based upon a passed in Rest Response
        /// </summary>
        /// <param name="response">The Rest Response which will form the basis of this Trigger Result</param>
        public TriggerResult(IRestResponse response) : base(response)
        {
            EventIdData eventIdData = null;
            try
            {
                eventIdData =  JsonConvert.DeserializeObject<EventIdData>(response.Content);
            }
            catch (Exception)
            {
                string msg =
                    string.Format("The response body from the Pusher HTTP endpoint could not be parsed as JSON: {0}{1}",
                                  Environment.NewLine,
                                  response.Content);
                throw new TriggerResponseException(msg);
            }

            _eventIds = new ReadOnlyDictionary<string, string>(eventIdData.event_ids);
        }

        /// <inheritDoc/>
        public IDictionary<string, string> EventIds
        {
            get { return this._eventIds; }
        }
    }
}