# Pusher .NET Server library

This is a .NET library for interacting with the Pusher REST API.

Registering at <http://pusher.com> and use the application credentials within your app as shown below.

Comprehensive documenation can be found at <http://pusher.com/docs/>.

## Installation

** NuGet Package coming soon **
```
// TODO
```

## How to use

### Constructor

```
var Pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET);
```

### Publishing/Triggering events

To trigger an event on one or more channels use the trigger function.

#### A single channel

```
pusher.Trigger( "channel-1", "test_event", new { message = "hello world" } );
```

#### Multiple channels

```
pusher.Trigger( new string[]{ "channel-1", "channel-2" ], "test_event", { message: "hello world" } );
```

### Excluding event recipients

In order to avoid the person that triggered the event also receiving it the `trigger` function can take an optional `ITriggerOptions` parameter which has a `SocketId` property. For more informaiton see: <http://pusher.com/docs/publisher_api_guide/publisher_excluding_recipients>.

```
pusher.Trigger(channel, event, data, new TriggerOptions() { SocketId = "1234.56" } );
```

### Authenticating Private channels

To authorise your users to access private channels on Pusher, you can use the `Authenticate` function:

```
var auth = pusher.Authenticate( socketId, channel );
```

For more information see: <http://pusher.com/docs/authenticating_users>

### Authenticating Presence channels

Using presence channels is similar to private channels, but you can specify extra data to identify that particular user:

```
var channelData = new PresenceChannelData() {
	user_id: "unique_user_id",
	user_info: new {
	  name = "Phil Leggetter"
	  twitter_id = "@leggetter"
	}
};
var auth = pusher.Authenticate( socketId, channel, channelData );
```

The `auth` is then returned to the caller as JSON.

For more information see: <http://pusher.com/docs/authenticating_users>

## License

This code is free to use under the terms of the MIT license.