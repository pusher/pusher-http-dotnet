using RestSharp;
using System.Web.Script.Serialization;
using System;
using System.Net;

namespace PusherServer
{
    /// <summary>
    /// Deserialised the result from a Rest Response
    /// </summary>
    /// <typeparam name="T">The Type the Rest Response contains</typeparam>
    public class GetResult<T> : RequestResult, IGetResult<T>
    {
        T _data;

        /// <summary>
        /// Attempts to deserialise the data contained with a Rest Response
        /// </summary>
        /// <param name="response">The response containing the data to deserialise</param>
        public GetResult(IRestResponse response):
            base(response)
        {
            try
            {
                _data = new JavaScriptSerializer().Deserialize<T>(response.Content);
            }
            catch (Exception e)
            {
                this.StatusCode = HttpStatusCode.BadRequest;
                this.Body = string.Format("The HTTP response could not be deserialized to the expected type. The following exception occurred: {0}", e);
            }
        }

        /// <summary>
        /// Gets the data deserialised from the Rest Response
        /// </summary>
        public T Data
        {
            get
            {
                return _data;
            }
        }
    }
}
