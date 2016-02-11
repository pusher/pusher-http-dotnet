using System;
using System.Net;
using RestSharp;

namespace PusherServer
{
    /// <summary>
    /// Deserialised the result from a Rest Response
    /// </summary>
    /// <typeparam name="T">The Type the Rest Response contains</typeparam>
    public class GetResult<T> : RequestResult, IGetResult<T>
    {
        /// <summary>
        /// Attempts to deserialise the data contained with a Rest Response
        /// </summary>
        /// <param name="response">The response containing the data to deserialise</param>
        /// <param name="deserializer"></param>
        public GetResult(IRestResponse response, IDeserializeJsonStrings deserializer) : base(response)
        {
            if (deserializer == null)
            {
                throw new ArgumentNullException("deserializer", "An instance of a deserializer needs to be provided");
            }

            DeserializeResponse(response, deserializer);
        }

        /// <summary>
        /// Gets the data deserialised from the Rest Response
        /// </summary>
        public T Data { get; private set; }

        private void DeserializeResponse(IRestResponse response, IDeserializeJsonStrings deserializer)
        {
            try
            {
                Data = deserializer.Deserialize<T>(response.Content);
            }
            catch (Exception e)
            {
                StatusCode = HttpStatusCode.BadRequest;
                Body = string.Format("The HTTP response could not be deserialized to the expected type. The following exception occurred: {0}", e);
            }
        }
    }
}
