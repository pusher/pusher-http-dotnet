using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PusherServer.Tests.UnitTests
{
    [TestClass]
    public class When_using_RawBodySerializer_to_serialize_a_rest_response
    {
        [TestMethod]
        public void RawBodySerialiser_should_return_an_empty_string_when_given_a_null_body()
        {
            var rawBodySerializer = new RawBodySerializer();
            var response = rawBodySerializer.Serialize(null);

            Assert.IsEmpty(response);
        }

        [TestMethod]
        public void RawBodySerializer_should_return_the_pass_in_string_when_it_is_populated()
        {
            var populatedString = "populated string";

            var rawBodySerializer = new RawBodySerializer();
            var response = rawBodySerializer.Serialize(populatedString);

            StringAssert.IsMatch(populatedString, response);
        }

        [TestMethod]
        public void RawBodySerializer_should_throw_an_exception_when_given_any_object_instead_of_a_string_to_serialize()
        {
            var anObject = new object();
            ArgumentException caughtException = null;

            try
            {
                var rawBodySerializer = new RawBodySerializer();
                rawBodySerializer.Serialize(anObject);
            }
            catch (ArgumentException ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException);
            StringAssert.IsMatch("The RawBodySerializer only supports strings for messages.  The body type was: ", caughtException.Message);
        }
    }
}
