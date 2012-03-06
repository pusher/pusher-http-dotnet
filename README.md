The Pusher REST .NET project is implementation of the Pusher REST API in C#.

It provides functionality to let you trigger events using the Pusher REST API in addition to 
[authenticating](http://pusher.com/docs/authenticating_users) user subscription request to
[private](http://pusher.com/docs/private_channels) and [presence](http://pusher.com/docs/presence) channels.

## Trigger events

Events can be triggered with any type of data which is then serialized to JSON.

	var provider = new PusherProvider(applicationId, applicationKey, applicationSecret);
	var request = new ObjectPusherRequest("test_channel", "my_event", new
																		{
																			some = "data"
																		});

	provider.Trigger(request);

## Authenticate Private Channels

Assuming that the running application is an ASP.NET app then the private channels are
authenticated using the `IPusherProvider.Authenticate` method with the values for the
channel name and the socket ID retrieved from the `Request` object.

	var provider = new PusherProvider(appId, appKey, appSecret);
	string auth = provider.Authenticate(Request["channel_name"], Request["socket_id"]);
	Response.Write(auth);

## Authenticate Presence Channels

Presence channels are authenticated in much the same way as private channels. The only difference
is that the channel data must also be passed to the `IPusherProvider.Authenticate` method so
that it can be hashed within the authentication string.

	var provider = new PusherProvider(appId, appKey, appSecret);
	var presenceChannelData = new PresenceChannelData()
        {
            user_id = "leggetter",
            user_info = new { name = "Phil Leggetter", twitter = "@leggetter" }
        };
	string auth = provider.Authenticate(Request["channel_name"], Request["socket_id"], presenceChannelData);
	Response.Write(auth);
