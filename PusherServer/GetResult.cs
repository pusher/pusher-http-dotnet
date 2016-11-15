using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
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
                throw new ArgumentNullException(nameof(deserializer), "An instance of a deserializer needs to be provided");
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

    /// <summary>
    /// Deserialised the result from a Rest Response
    /// </summary>
    /// <typeparam name="T">The Type the Rest Response contains</typeparam>
    public class GetResult2<T> : RequestResult2, IGetResult<T>
    {
        /// <summary>
        /// Attempts to deserialise the data contained with a Rest Response
        /// </summary>
        /// <param name="response">The original response from the rest call</param>
        /// <param name="content">the extracted content</param>
        public GetResult2(HttpResponseMessage response, string content) : base(response, content)
        {
            Data = JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Gets the data deserialised from the Rest Response
        /// </summary>
        public T Data { get; private set; }
    }
}
