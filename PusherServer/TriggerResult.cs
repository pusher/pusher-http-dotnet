using PusherServer.Exceptions;
using PusherServer.Util;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace PusherServer
{
    internal class TriggerResult: RequestResult, ITriggerResult
    {
        private readonly IDictionary<string, string> _eventIds;

        public TriggerResult(IRestResponse response):
            base(response)
        {
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var msg = string.Format("The response from the Pusher HTTP API was not 200 OK. It was {0}", response.StatusCode);
                throw new TriggerResponseException(msg);
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                throw new TriggerResponseException("The response body from the Pusher HTTP API was either null or empty");
            }

            EventIdData eventIdData = null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                eventIdData = serializer.Deserialize<EventIdData>(response.Content);
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

        public IDictionary<string, string> EventIds
        {
            get
            {
                return this._eventIds;
            }
        }
    }
}
