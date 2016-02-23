# Changelog

## 3.0.0 rc4

* [ADDED] Support for the `cluster` option.

## 3.0.0 rc3

* [ADDED] Build support for the Pusher Travis instance
* [CHANGED] Trigger based calls that result in a bad response no longer throw TriggerResponseExceptions. The response and original content are now available as properties for interrogation
* [ADDED] The Pusher HTTP API Host & Port can now be set via PusherOptions.HostName & PusherOptions.Port properties
* [ADDED] TriggerAsync to allow asynchronous requests to be made to the HTTP API.
* [ADDED] New API abstractions onto Pusher for fetching info on single and multiple channels, and fetching users from a presence Channel. Both sync and sync calls are supported.
* [REMOVED] Event Buffer support
* [ADDED] Support for providing a different JSON serializer & deserializer

## 3.0.0 rc1/2

* [CHANGED] `Trigger` calls that result in a non 200 response from the Pusher HTTP API now result in a `TriggerResponseException` being thrown.
			This is a BREAKING CHANGE as previously you could inspect the `ITriggerResult.StatusCode` to detect a failed request.
* [ADDED]	When triggering events against a Pusher cluster that supports Event Buffer functionality the IDs of the triggered events
			can be retrieved via `ITriggerResult.EventIds`
* [ADDED]	The Pusher HTTP API Host can now be set via `PusherOptions.Host`
* [ADDED]	`TriggerAsync` to allow asynchronous requests to be made to the HTTP API.

## 2.1.1

* [FIXED] Channel name and socket_id values are validated for `Pusher.Trigger`
* [FIXED] socket_id values are validated for `Pusher.Authenticate`

## 2.1.0

* [FIXED] Pusher.Authenticate for private channels. Should not return `channel_data` in the JSON.
* [CHANGED] IAuthenticationData Pusher.Authenticate(string channelName, string socketId, PresenceChannelData presenceData) to throw a ArgumentNullException if `presenceData` is `null`.

## 2.0.0

* Full release of 2.0.0 library
* Added `Pusher.ProcessWebHook(signature, body)` support

## 2.0.0-beta-5

* Updating RestSharp to 105.0.1
