# Pusher Channels .NET HTTP API library

[![NuGet Badge](https://buildstats.info/nuget/pusherserver)](https://www.nuget.org/packages/PusherServer/)
![Build](https://github.com/pusher/pusher-http-dotnet/workflows/Build/badge.svg)

This is a .NET library for interacting with the Pusher Channels HTTP API.

Registering at <http://pusher.com/channels> and use the application credentials within your app as shown below.

Comprehensive documentation can be found at <http://pusher.com/docs/channels>.

## Supported platforms
- .NET Standard 1.3
- .NET Standard 2.0
- .NET 4.5
- .NET 4.7.2
- Unity 2018.1

**Note:** from release 4.4.0 PusherServer.Core.dll has been removed. Applications should reference PusherServer.dll instead.

 ## Contents

- [Installation](#installation)
- [Getting started](#getting-started)
- [Configuration](#configuration)
- [Triggering events](#triggering-events)
  - [Single channel](#single-channel)
  - [Multiple channels](#multiple-channels)
  - [Batches](#batches)
  - [Detecting event data that exceeds the 10KB threshold](#detecting-event-data-that-exceeds-the-10kb-threshold)
  - [Excluding event recipients](#excluding-event-recipients)
- [Authenticating channel subscription](#authenticating-channel-subscription)
  - [Authenticating Private channels](#authenticating-private-channels)
  - [Authenticating Presence channels](#authenticating-presence-channels)
- [End-to-end encryption](#end-to-end-encryption)
- [Querying application state](#querying-application-state)
  - [Getting information for all channels](#getting-information-for-all-channels)
  - [Getting information for a channel](#getting-information-for-a-channel)
  - [Getting user information for a presence channel](#getting-user-information-for-a-presence-channel)
- [Webhooks](#webhooks)
- [Developer notes](#developer-notes)
  - [Debug tracing](#debug-tracing)
  - [Asynchronous programming](#asynchronous-programming)
  - [Alternative environments](#alternative-environments)
- [License](#license)

## Installation

The compiled library is available on NuGet:

```
Install-Package PusherServer
```

## Getting started

The minimum configuration required to use the `Pusher` object are the three
constructor arguments which identify your Pusher app - app id, app key and app secret.
You can find them by going to "App Keys" on your app at <https://dashboard.pusher.com/apps>.
If your app is not in the default cluster "mt1", you can specify it via the `PusherOptions` object.


```cs
var options = new PusherOptions
{
    Cluster = APP_CLUSTER,
    Encrypted = true,
};

var pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET, options);
```

**Please Note:** the `Cluster` option is overridden by `HostName` option. So, if `HostName` is set then `Cluster` will be ignored.

## Configuration

In addition to the three app identifiers - app id, app key and app secret needed when constructing a Pusher object;
you can specify other options via the `PusherOptions` object:

| Property                    | Type              | Description                                                                                                                                                        |
|-----------------------------|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Cluster                     | String            | The Pusher app cluster name; for example, "eu". The default value is "mt1". This value will be overridden by `HostName`.                                           |
| HostName                    | String            | The Pusher app host name excluding the scheme; for example, "api.pusherapp.com". Overrides `Cluster` if specified.                                                 |
| Encrypted                   | Boolean           | Indicates whether calls to the Pusher REST API are over HTTP or HTTPS. The default value is **false** - communication over HTTP.                                   |
| Port                        | Integer           | The REST API port that the HTTP calls will be made to. If `Encrypted` is **true**, will default to port 443. If `Encrypted` is **false**, will default to port 80. |
| BatchEventDataSizeLimit     | Nullable Integer  | Optional size limit for the `Data` property of a triggered event. If specified, the size check is done client side before submitting the event to the server. The size limit is normally 10KB but SDK customers can request a larger limit. |
| EncryptionMasterKey         | Byte Array        | Optional 32 byte encryption key required for end-to-end encryption of private channels.                                                                            |
| RestClientTimeout           | TimeSpan          | The Pusher REST API timeout. The default timeout is 100 seconds.                                                                                                   |
| TraceLogger                 | ITraceLogger      | Used for tracing diagnostic events. Should not be set in production code.                                                                                          |

## Triggering events

To trigger an event on one or more channels use the `TriggerAsync` function.

### Single channel

```cs
ITriggerResult result = await pusher.TriggerAsync("channel-1", "test_event", new
{
    message = "hello world"
}).ConfigureAwait(false);
```

### Multiple channels

```cs
ITriggerResult result = await pusher.TriggerAsync(
    new string[] 
    { 
        "channel-1", "channel-2"
    },
    "test_event",
    new
    {
        message = "hello world"
    }).ConfigureAwait(false);
```

### Batches

```cs
var events = new[]
{
    new Event {Channel = "channel-1", EventName = "event-1", Data = "hello world"},
    new Event {Channel = "channel-2", EventName = "event-2", Data = "my name is bob"}
};

ITriggerResult result = await pusher.TriggerAsync(events).ConfigureAwait(false);
```

### Detecting event data that exceeds the 10KB threshold

Rather than relying on the server to validate message size you can now perform this client side before submitting a trigger event. Here is an example on how to do this:

```cs
IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
{
    HostName = Config.HttpHost,
    BatchEventDataSizeLimit = PusherOptions.DEFAULT_BATCH_EVENT_DATA_SIZE_LIMIT, // 10KB
});

try
{
    var events = new[]
    {
        new Event {Channel = "channel-1", EventName = "event-1", Data = "hello world"},
        new Event {Channel = "channel-2", EventName = "event-2", Data = new string('Q', 10 * 1024 + 1)},
    };
    await pusher.TriggerAsync(events).ConfigureAwait(false);
}
catch (EventDataSizeExceededException eventDataSizeError)
{
    // Handle the error when event data exceeds 10KB
}
```

### Excluding event recipients

In order to avoid the person that triggered the event also receiving it the `trigger` function can take an optional `ITriggerOptions` parameter which has a `SocketId` property. For more information see: <https://pusher.com/docs/channels/server_api/excluding-event-recipients>.

```cs
ITriggerResult result = await pusher.TriggerAsync(channel, event, data, new TriggerOptions
{
    SocketId = "1234.56"
}).ConfigureAwait(false);
```

## Authenticating channel subscription

### Authenticating Private channels

To authorise your users to access private channels on Channels, you can use the `Authenticate` function:

```cs
var auth = pusher.Authenticate(channelName, socketId);
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Channels.

For more information see: <https://pusher.com/docs/channels/server_api/authenticating-users>

### Authenticating Presence channels

Using presence channels is similar to private channels, but you can specify extra data to identify that particular user:

```cs
var channelData = new PresenceChannelData
{
    user_id = "unique_user_id",
    user_info = new
    {
        name = "Phil Leggetter",
        twitter_id = "@leggetter",
    }
};
var auth = pusher.Authenticate(channelName, socketId, channelData);
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Channels.

For more information see: <https://pusher.com/docs/channels/server_api/authenticating-users>

## End-to-end encryption

This library supports end-to-end encryption of your private channels. This means that only you and your connected clients
will be able to read your messages.

More information on end-to-end encrypted channels can be found [here](https://pusher.com/docs/client_api_guide/client_encrypted_channels).

**Please note:** Encrypted channels must be prefixed with `private-encrypted-`. Currently, only private channels can be encrypted.
See [channel naming conventions](https://pusher.com/docs/channels/using_channels/channels#channel-naming-conventions).

You can enable this feature by following these steps:

1. You should first set up Private channels. This involves
   [creating an authentication endpoint on your server](https://pusher.com/docs/authenticating_users).

2. Next, generate a 32 byte master encryption key.

   Because it is a secret, store this key securely and do not share it with anyone, not even Pusher.

   To generate a suitable key from a secure random source, you could use `System.Security.Cryptography.RandomNumberGenerator`:

   ```cs
   byte[] encryptionMasterKey = new byte[32];
   using (RandomNumberGenerator random = RandomNumberGenerator.Create())
   {
       random.GetBytes(encryptionMasterKey);
   }
   ```
3. Pass your master encryption key to the SDK constructor

   ```cs
   Pusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions
   {
       Cluster = Config.Cluster,
       EncryptionMasterKey = encryptionMasterKey,
       Encrypted = true,
   });

   await pusher.TriggerAsync("private-encrypted-my-channel", "my-event", new 
   {
       message = "hello world"
   }).ConfigureAwait(false);
   ```
4. Subscribe to these channels in your client, and you're done!
   You can verify it is working by checking out the debug console on the
   <https://dashboard.pusher.com> and seeing the scrambled ciphertext.

## Querying application state

It is possible to query the state of your Pusher application using the generic `Pusher.GetAsync( resource )` method and overloads.

For full details see: <https://pusher.com/docs/channels/library_auth_reference/rest-api>

### Getting information for all channels

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
IGetResult<ChannelsList> result = await _pusher.GetAsync<ChannelsList>(
    "/channels",
    new
    {
        filter_by_prefix = "presence-"
    }).ConfigureAwait(false);
```

or

```cs
IGetResult<ChannelsList> result = await pusher.FetchStateForChannelsAsync<ChannelsList>(new
{
    filter_by_prefix = "presence-"
}).ConfigureAwait(false);
```

### Getting information for a channel

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

### Getting user information for a presence channel

Retrieve a list of users that are on a presence channel:

```cs
IGetResult<object> result =
    await pusher.FetchUsersFromPresenceChannelAsync<object>("/channels/presence-channel/users");
```

or

```cs
IGetResult<object> result =
    await pusher.FetchUsersFromPresenceChannelAsync<object>("my_channel");
```

*Note: `object` has been used above because as yet there isn't a defined class that the information can be serialized on to*

## Webhooks

Channels will trigger Webhooks based on the settings you have for your application. You can consume these and use them
within your application as follows.

For more information see <https://pusher.com/docs/channels/server_api/webhooks>.

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
  // The Webhook validated
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

## Developer notes

* Developed using Visual Studio 2017 Community Edition
* PusherServer acceptance tests depends on [PusherClient](https://github.com/pusher-community/pusher-websocket-dotnet).

The Pusher test application settings are now loaded from a JSON config file stored in the root of the source tree and named `AppConfig.test.json`.
Make a copy of `./AppConfig.sample.json` and name it `AppConfig.test.json`.
Modify the contents of `AppConfig.test.json` with your test application settings.
You should be good to run all the tests successfully.

### Debug tracing

Debug tracing is now off by default. To enable it use the new Pusher option: TraceLogger.

```cs
IPusher pusher = new Pusher(Config.AppId, Config.AppKey, Config.AppSecret, new PusherOptions()
{
    HostName = Config.HttpHost,
    TraceLogger = new DebugTraceLogger(),
});
```
### Asynchronous programming

From v4.0.0 onwards, this library uses the `async` / `await` [syntax](https://msdn.microsoft.com/en-gb/library/mt674882.aspx) from .NET 4.5+.

This means that you can now use the Channels .NET library asynchronously using the following code style:

```cs
using PusherServer;

var options = new PusherOptions();
options.Cluster = APP_CLUSTER;

var pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET, options);

Task<ITriggerResult> resultTask = pusher.TriggerAsync("my-channel", "my-event", new
{
    message = "hello world"
});

// You can do work here that doesn't rely on the result of TriggerAsync  
DoIndependentWork();

ITriggerResult result = await resultTask;
```

This also means that the library is now only officially compatible with .NET 4.5 and above (including .NET Core). If you need to support older versions of the .NET framework then you have a few options:

* Use a previous version of the library, such as [v3.0.0](https://www.nuget.org/packages/PusherServer/3.0.0)
* Use a workaround package such as [Microsoft Async](https://www.nuget.org/packages/Microsoft.Bcl.Async) or [AsyncBridge](https://www.nuget.org/packages/AsyncBridge.Net35).

Please note that neither of these workarounds will be officially supported by Pusher.

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

## License

This code is free to use under the terms of the MIT license.
