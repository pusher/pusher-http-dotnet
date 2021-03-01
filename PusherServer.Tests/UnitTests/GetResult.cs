using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PusherServer.Tests.UnitTests
{
    [TestClass]
    public class When_using_GetResult_to_deserialise_a_rest_response
    {
        [TestMethod]
        public void GetResult_should_gracefully_handle_a_deserialisation_exception()
        {
            var stubRestResponse = Substitute.For<HttpResponseMessage>();
            stubRestResponse.Content = new StringContent("not json");
            stubRestResponse.StatusCode = HttpStatusCode.BadRequest;

            var getResult = new GetResult<object>(stubRestResponse, "not json");

            Assert.AreEqual(HttpStatusCode.BadRequest, getResult.StatusCode);
            StringAssert.IsMatch("not json", getResult.Body);
            Assert.IsNull(getResult.Data);
            Assert.AreEqual(stubRestResponse, getResult.Response);
        }

        [TestMethod]
        public void GetResult_should_deserialise_a_valid_json_response()
        {
            var stubRestResponse = Substitute.For<HttpResponseMessage>();
            stubRestResponse.Content = new StringContent("[\"string1\", \"string2\", \"string3\"]");

            var getResult = new GetResult<List<string>>(stubRestResponse, "[\"string1\", \"string2\", \"string3\"]");

            Assert.AreNotEqual(HttpStatusCode.BadRequest, getResult.StatusCode);
            Assert.AreEqual(3, getResult.Data.Count);
            StringAssert.IsMatch("string2", getResult.Data[1]);
            Assert.AreEqual(stubRestResponse, getResult.Response);
        }
    }
}
