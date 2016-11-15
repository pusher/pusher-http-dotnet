using System;
using NUnit.Framework;
using PusherServer.RestfulClient;
using PusherServer.Tests.RestfulClient.Fakes;

namespace PusherServer.Tests.RestfulClient
{
    [TestFixture]
    public class When_making_a_request
    {
        [Test]
        public async void then_the_get_request_should_be_made_with_a_valid_resource()
        {
            var factory = new AuthenticatedRequestFactory(Config.AppKey, Config.AppId, Config.AppSecret);
            var request = factory.Build(PusherMethod.GET, "/channels/newRestClient");

            var client = new PusherRestClient("http://api.pusherapp.com", "pusher-http-dotnet", Version.Parse("4.0.0"));
            var response = await client.ExecuteAsync<TestOccupied>(request);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Data.Occupied);
        }

        //[Test]
        //public async void then_the_post_request_should_be_made_with_a_valid_resource()
        //{
        //    var factory = new AuthenticatedRequestFactory(Config.AppKey, Config.AppId, Config.AppSecret);

        //    var testObject = new { hello = "world" };

        //    var request = factory.Build(PusherMethod.POST, "/trigger", requestBody: testObject);

        //    var client = new PusherRestClient("http://api.pusherapp.com");
        //    var response = await client.ExecuteAsync<object>(request, "pusher-http-dotnet", Version.Parse("3.0.0"));

        //    Assert.IsNotNull(response);
        //}
    }
}