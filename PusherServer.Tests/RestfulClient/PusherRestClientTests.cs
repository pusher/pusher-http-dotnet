using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PusherServer.RestfulClient;
using PusherServer.Tests.RestfulClient.Fakes;

namespace PusherServer.Tests.RestfulClient
{
    [TestClass]
    public class When_making_a_request
    {
        [TestMethod]
        public async Task then_the_get_request_should_be_made_with_a_valid_resource()
        {
            var factory = new AuthenticatedRequestFactory(Config.AppKey, Config.AppId, Config.AppSecret);
            var request = factory.Build(PusherMethod.GET, "/channels/newRestClient");

            var client = new PusherRestClient($"http://{Config.HttpHost}", "pusher-http-dotnet", Version.Parse("4.0.0"));
            var response = await client.ExecuteGetAsync<TestOccupied>(request);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data.Occupied);
        }
    }
}