using PusherServer.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace PusherServer
{
    internal class NotifyResult : RequestResult, INotifyResult
    {
        private readonly int _numberOfSubscribers;

        /// <summary>
        /// Constructs a new instance of a NotifyResult based upon a passed in Rest Response
        /// </summary>
        /// <param name="response">The Rest Response which will form the basis of this Notify Result</param>
        public NotifyResult(IRestResponse response) : base(response)
        {
            NotifySubscriberData subscriberData = null;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                subscriberData = serializer.Deserialize<NotifySubscriberData>(response.Content);
            }
            catch (Exception)
            {
                string msg =
                    string.Format("The response body from the Pusher HTTP endpoint could not be parsed as JSON: {0}{1}",
                                  Environment.NewLine,
                                  response.Content);
                throw new NotifyResponseException(msg);
            }

            _numberOfSubscribers = subscriberData.number_of_subscribers;
        }

        /// <inheritDoc/>
        public int NumberOfSubscribers
        {
            get { return this._numberOfSubscribers; }
        }
    }
}
