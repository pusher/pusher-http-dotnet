using NSubstitute;
using NUnit.Framework;
using PusherServer.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void it_should_not_accept_a_null_response()
        {
            new TriggerResult(null);
        }

        [Test]
        [ExpectedException(typeof(TriggerResponseException))]
        public void it_should_treat_an_empty_response_as_a_failed_request()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = "";
            var triggerResult = new TriggerResult(response);
        }

        [Test]
        [ExpectedException(typeof(TriggerResponseException))]
        public void it_should_treat_an_empty_JSON_response_as_a_failed_request()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = "{}";
            var triggerResult = new TriggerResult(response);
        }

        [Test]
        [ExpectedException(typeof(TriggerResponseException))]
        public void it_should_treat_a_JSON_response_with_no_event_ids_as_a_failed_request()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = "{\"event_ids\":{}}";
            var triggerResult = new TriggerResult(response);
        }
        
        [Test]
        public void it_should_parse_the_returned_JSON()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = TriggerResultHelper.TRIGGER_RESPONSE_JSON;
            var triggerResult = new TriggerResult(response);

            Assert.AreEqual(3, triggerResult.EventIds.Count);
        }

        [Test]
        public void it_should_expose_the_event_id_values_for_each_channel()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = TriggerResultHelper.TRIGGER_RESPONSE_JSON;
            var triggerResult = new TriggerResult(response);

            Assert.AreEqual("ch1_event_id", triggerResult.EventIds["ch1"]);
            Assert.AreEqual("ch2_event_id", triggerResult.EventIds["ch2"]);
            Assert.AreEqual("ch3_event_id", triggerResult.EventIds["ch3"]);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void it_should_not_be_possible_to_change_EventIds()
        {
            IRestResponse response = Substitute.For<IRestResponse>();
            response.Content = TriggerResultHelper.TRIGGER_RESPONSE_JSON;
            var triggerResult = new TriggerResult(response);

            triggerResult.EventIds.Add("fish", "pie");
            
        }

    }
}
