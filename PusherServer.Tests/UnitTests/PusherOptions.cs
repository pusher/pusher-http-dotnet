using NSubstitute;
using NUnit.Framework;
using System;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_creating_a_new_PusherOptions_instance
    {
        [Test]
        public void a_default_RestClient_should_be_used_if_one_is_not_set_on_PusherOptions_parameter()
        {
            var options = new PusherOptions();
            Assert.IsNotNull(options.RestClient);
        }
    }
}