using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PusherServer.Tests.UnitTests
{
    [TestClass]
    public class When_deserializing_to_a_ChannelsList
    {
        string json = "{\"channels\":" +
                            "{" +
                                "\"test_channel\": {}," +
                                "\"presence-channel\": { \"user_count\": \"300\" } " +
                            "}" +
                       "}";

        [TestMethod]
        public void It_should_make_the_channels_accessible_by_name_via_the_indexer()
        {
            ChannelsList list = JsonConvert.DeserializeObject<ChannelsList>(json);

            Assert.IsNotNull(list["test_channel"]);
        }

        [TestMethod]
        public void It_should_be_possible_to_access_channel_information()
        {
            ChannelsList list = JsonConvert.DeserializeObject<ChannelsList>(json);

            Assert.AreEqual( list["presence-channel"]["user_count"], "300" );
        }
    }
}
