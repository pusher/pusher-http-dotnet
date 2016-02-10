using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using PusherServer.Tests.Helpers;

namespace PusherServer.Tests.AcceptanceTests
{
    [TestFixture]
    public class When_authenticating_a_private_subscription
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            PusherClient.Pusher.Trace.Listeners.Add(new ConsoleTraceListener(true));
        }

        [Test]
        public void the_authentication_token_for_a_private_channel_should_be_accepted_by_Pusher()
        {
            PusherServer.Pusher pusherServer = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });
            PusherClient.Pusher pusherClient =
                new PusherClient.Pusher(Config.AppKey, new PusherClient.PusherOptions()
                    {
                        Authorizer = new InMemoryAuthorizer(pusherServer)
                    });
            pusherClient.Host = Config.WebSocketHost;

            string channelName = "private-channel";

            bool subscribed = false;
            AutoResetEvent reset = new AutoResetEvent(false);

            pusherClient.Connected += new PusherClient.ConnectedEventHandler(delegate(object sender)
            {
                Debug.WriteLine("connected");
                reset.Set();
            });

            Debug.WriteLine("connecting");
            pusherClient.Connect();

            Debug.WriteLine("waiting to connect");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Debug.WriteLine("subscribing");
            var channel = pusherClient.Subscribe(channelName);
            channel.Subscribed += new PusherClient.SubscriptionEventHandler(delegate(object s)
            {
                Debug.WriteLine("subscribed");
                subscribed = true;
                reset.Set();
            });

            Debug.WriteLine("waiting to subscribe");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(subscribed);
        }

        [Test]
        public void the_authentication_token_for_a_presence_channel_should_be_accepted_by_Pusher()
        {
            PusherServer.Pusher pusherServer = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
            {
                HostName = Config.HttpHost
            });
            PusherClient.Pusher pusherClient =
                new PusherClient.Pusher(Config.AppKey, new PusherClient.PusherOptions()
                {
                    Authorizer = new InMemoryAuthorizer(
                        pusherServer,
                        new PresenceChannelData()
                        {
                            user_id = "leggetter",
                            user_info = new { twitter_id = "@leggetter" }
                        })
                });
            pusherClient.Host = Config.WebSocketHost;

            string channelName = "presence-channel";

            bool subscribed = false;
            AutoResetEvent reset = new AutoResetEvent(false);

            pusherClient.Connected += new PusherClient.ConnectedEventHandler(delegate(object sender)
            {
                Debug.WriteLine("connected");
                reset.Set();
            });

            Debug.WriteLine("connecting");
            pusherClient.Connect();

            Debug.WriteLine("waiting to connect");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Debug.WriteLine("subscribing");
            var channel = pusherClient.Subscribe(channelName);
            channel.Subscribed += new PusherClient.SubscriptionEventHandler(delegate(object s)
            {
                Debug.WriteLine("subscribed");
                subscribed = true;
                reset.Set();
            });

            Debug.WriteLine("waiting to subscribe");
            reset.WaitOne(TimeSpan.FromSeconds(5));

            Assert.IsTrue(subscribed);
        }
    }
}
