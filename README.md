# Pusher .NET HTTP API library

[![Build Status](https://travis-ci.org/pusher/pusher-http-dotnet.svg?branch=master)](https://travis-ci.org/pusher/pusher-http-dotnet)

This is a .NET library for interacting with the Pusher HTTP API.

Registering at <http://pusher.com> and use the application credentials within your app as shown below.

Comprehensive documentation can be found at <http://pusher.com/docs/>.

## Installation

### NuGet Package
```
Install-Package PusherServer
```

## How to use

### Constructor

```cs
var pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET);
```

If you created your app in a different cluster to the default cluster, specify this as follows:

```cs

var options = new PusherOptions();
options.Cluster = "eu";

var pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET, options);
```

*Please Note: the `Cluster` option is overridden by `HostName` option. So, if `HostName` is set then `Cluster` will be ignored.*

### Publishing/Triggering events

To trigger an event on one or more channels use the trigger function.

#### A single channel

```cs
ITriggerResult result = await pusher.TriggerAsync( "channel-1", "test_event", new { message = "hello world" } );
```

#### Multiple channels

```cs
ITriggerResult result = await pusher.TriggerAsync( new string[]{ "channel-1", "channel-2" ], "test_event", new { message: "hello world" } );
```

#### Batches

```cs
var events = new List[]{
  new Event(){ EventName = "test_event", Channel = "channel-1", Data = "hello world" },
  new Event(){ EventName = "test_event", Channel = "channel-1", Data = "my name is bob" },
}

ITriggerResult result = await pusher.TriggerAsync(events)
```

### Excluding event recipients

In order to avoid the person that triggered the event also receiving it the `trigger` function can take an optional `ITriggerOptions` parameter which has a `SocketId` property. For more information see: <http://pusher.com/docs/publisher_api_guide/publisher_excluding_recipients>.

```cs
ITriggerResult result = await pusher.TriggerAsync(channel, event, data, new TriggerOptions() { SocketId = "1234.56" } );
```

### Authenticating Private channels

To authorise your users to access private channels on Pusher, you can use the `Authenticate` function:

```cs
var auth = pusher.Authenticate( channelName, socketId );
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Pusher.

For more information see: <http://pusher.com/docs/authenticating_users>

### Authenticating Presence channels

Using presence channels is similar to private channels, but you can specify extra data to identify that particular user:

```cs
var channelData = new PresenceChannelData() {
	user_id: "unique_user_id",
	user_info: new {
	  name = "Phil Leggetter"
	  twitter_id = "@leggetter"
	}
};
var auth = pusher.Authenticate( channelName, socketId, channelData );
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Pusher.

For more information see: <http://pusher.com/docs/authenticating_users>

### Application State

It is possible to query the state of your Pusher application using the generic `Pusher.GetAsync( resource )` method and overloads.

For full details see: <http://pusher.com/docs/rest_api>

#### List channels

You can get a list of channels that are present within your application:

```cs
IGetResult<ChannelsList> result = await pusher.GetAsync<ChannelsList>("/channels");
```

or

```cs
IGetResult<ChannelsList> result = await pusher.FetchStateForChannelsAsync<ChannelsList>();
```

You can provide additional parameters to filter the list of channels that is returned.

```cs
IGetResult<ChannelsList> result = await pusher.GetAsync<ChannelsList>("/channels", new { filter_by_prefix = "presence-" } );
```

or

```cs
IGetResult<ChannelsList> result = await pusher.FetchStateForChannelsAsync<ChannelsList>(new { filter_by_prefix = "presence-" } );
```

#### Fetch channel information

Retrieve information about a single channel:

```cs
IGetResult<object> result = await pusher.GetAsync<object>("/channels/my_channel" );
```

or

```cs
IGetResult<object> result = await pusher.FetchStateForChannelAsync<object>("my_channel");
```

Retrieve information about multiple channels:

```cs
IGetResult<object> result = await pusher.FetchStateForChannelsAsync<object>();
```

*Note: `object` has been used above because as yet there isn't a defined class that the information can be serialized on to*

#### Fetch a list of users on a presence channel

Retrieve a list of users that are on a presence channel:

```cs
IGetResult<object> result = await pusher.FetchUsersFromPresenceAsync<object>("/channels/presence-channel/users" );
```

or

```cs
IGetResult<object> result = await pusher.FetchUsersFromPresenceChannelAsync<object>("my_channel");
```

*Note: `object` has been used above because as yet there isn't a defined class that the information can be serialized on to*

### WebHooks

Pusher will trigger WebHooks based on the settings you have for your application. You can consume these and use them
within your application as follows.

For more information see <https://pusher.com/docs/webhooks>.

```cs
// How you get these depends on the framework you're using

// HTTP_X_PUSHER_SIGNATURE from HTTP Header
var receivedSignature = "value";

// Body of HTTP request
var receivedBody = "value";

var pusher = new Pusher(...);
var webHook = pusher.ProcessWebHook(receivedSignature, receivedBody);
if(webHook.IsValid)
{
  // The WebHook validated
  // Dictionary<string,string>[]
  var events = webHook.Events;

  foreach(var webHookEvent in webHook.Events)
  {
    var eventType = webHookEvent["name"];
    var channelName = webHookEvent["channel"];

    // depending on the type of event (eventType)
    // there may be other values in the Dictionary<string,string>
  }

}
else {
  // Log the validation errors to work out what the problem is
  // webHook.ValidationErrors
}
```

### Asynchronous programming

From v4.0.0 onwards, this library uses the `async` / `await` [syntax](https://msdn.microsoft.com/en-gb/library/mt674882.aspx) from .NET 4.5+.

This means that you can now use the Pusher .NET library asynchronously using the following code style:

```
using PusherServer;

var pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET);

Task<ITriggerResult> resultTask = pusher.TriggerAsync( "my-channel", "my-event", new { message = "hello world" } );

// You can do work here that doesn't rely on the result of TriggerAsync  
DoIndependentWork();

ITriggerResult result = await resultTask;
```

This also means that the library is now only officially compatible with .NET 4.5 and above (including .NET Core). If you need to support older versions of the .NET framework then you have a few options:

* Use a previous version of the library, such as [v3.0.0](https://www.nuget.org/packages/PusherServer/3.0.0)
* Use a workaround package such as [Microsoft Async](https://www.nuget.org/packages/Microsoft.Bcl.Async) or AsyncBridge (https://www.nuget.org/packages/AsyncBridge.Net35).

Please note that neither of these workarounds will be officially supported by Pusher.

## Development Notes

* Developed using Visual Studio Community 2015
* The NUnit test framework is used for testing, your copy of Visual Studio needs the "NUnit test adapter" installed from Tools -> Extensions and Updates if you wish to run the test from the IDE.
* PusherServer acceptance tests depends on [PusherClient](https://github.com/pusher-community/pusher-websocket-dotnet).
* PusherServer has two variations, the original version for .NET, and a .NET Core version.  The source files all leave within the .NET Core folder, with links from the .NET project to these files to create the .NET version.

### Alternative environments

The solution can be opened and compiled in Xamarin Studio on OSX.

Alternatively, the solution can be built from the command line if Mono is installed.  First of all, open up a terminal and navigate to the root directory of the solution. The second step is to restore the Nuget packages, which can be done with this command

```sh
nuget restore pusher-dotnet-server.sln
```

and finally build the solution, now that the packages have been restored

```sh
xbuild pusher-dotnet-server.sln
```

During the build, there will be a warning about a section called TestCaseManagementSettings in the GlobalSection.  Please ignore this, as it is a Visual Studio specific setting.

#### Different solutions in this repository

There are 3 solution files in this repository.  One for just .NET, one for just .NET Core and a third one with both the platforms in.

The source files for this project can be found under the .NET Core project.  These files are then linked to in the .NET project to allow creation for both platforms.

## Publish to NuGet

You should be familiar with [creating and publishing NuGet packages](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).

From the `pusher-dotnet-server` directory:

1. Update `./PusherServer/Properties/AssemblyInfo.cs` and `./PusherServer.Core/Properties/AssemblyInfo.cs` with new version number.
2. Check and change any info required in `PusherServer/PusherServer.nuspec`.
3. Run `package.cmd` to pack a package to deploy to NuGet.
3. Run `tools/nuget.exe push PusherServer.{VERSION}.nupkg'.

## License

This code is free to use under the terms of the MIT license.
