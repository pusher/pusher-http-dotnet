using RestSharp;
using System.Web.Script.Serialization;
using System;
using System.Net;
namespace PusherServer
{
    public class GetResult<T>: RequestResult, IGetResult<T>
    {
        T _data;

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

        public T Data
        {
            get
            {
                return _data;
            }
        }
    }
}
