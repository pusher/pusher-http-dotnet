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
            EventIdData eventIdData = null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                eventIdData = serializer.Deserialize<EventIdData>(response.Content);
            }
            catch (Exception)
            {
                // Allow this to be swallowed. It will be caught in the following (eventIdData == null) check.
            }

            if (eventIdData == null || eventIdData.event_ids == null || eventIdData.event_ids.Count == 0)
            {
                string msg =
                    string.Format("The response from the Pusher HTTP endpoint did not provide the expect event_ids. The response was: {0}{1}",
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
