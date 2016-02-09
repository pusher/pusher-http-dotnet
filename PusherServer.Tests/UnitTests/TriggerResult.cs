using System;
using System.Net;
using NSubstitute;
using NUnit.Framework;
using PusherServer.Exceptions;
using RestSharp;

namespace PusherServer.Tests.UnitTests
{
    public class TriggerResultHelper
    {
        public static string TRIGGER_RESPONSE_JSON = "{" +
                            "\"event_ids\": {" +
                                "\"ch1\": \"ch1_event_id\"," +
                                "\"ch2\": \"ch2_event_id\"," +
                                "\"ch3\": \"ch3_event_id\"" +
                            "}" +
                        "}";

    }

    [TestFixture]
    public class When_initialisation_a_TriggerEvent
    {
        private IRestResponse V7_PROTOCOL_SUCCESSFUL_RESPONSE;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            V7_PROTOCOL_SUCCESSFUL_RESPONSE = Substitute.For<IRestResponse>();
            V7_PROTOCOL_SUCCESSFUL_RESPONSE.Content = "{}";
            V7_PROTOCOL_SUCCESSFUL_RESPONSE.StatusCode = HttpStatusCode.OK;
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void it_should_not_accept_a_null_response()
        {
            new TriggerResult(null);
        }

        [Test]
        public void it_should_treat_a_v7_protocol_200_response_as_a_successful_request()
        {
            var triggerResult = new TriggerResult(V7_PROTOCOL_SUCCESSFUL_RESPONSE);
            Assert.AreEqual(HttpStatusCode.OK, triggerResult.StatusCode);
        }

        [Test]
        public void it_should_have_no_event_id_value_when_a_v7_protocol_200_response_is_returned()
        {
            var triggerResult = new TriggerResult(V7_PROTOCOL_SUCCESSFUL_RESPONSE);
            Assert.AreEqual(0, triggerResult.EventIds.Count);
        }

        [Test]
        [ExpectedException(typeof (TriggerResponseException))]
        public void it_should_treat_non_JSON_content_in_the_request_body_as_a_failed_request()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = "FISH";
            response.StatusCode = HttpStatusCode.OK;

            new TriggerResult(response);
        }

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void it_should_not_be_possible_to_change_EventIds()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = TriggerResultHelper.TRIGGER_RESPONSE_JSON;
            var triggerResult = new TriggerResult(response);

            triggerResult.EventIds.Add("fish", "pie");
        }
    }
}