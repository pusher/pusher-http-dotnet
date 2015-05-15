# Changelog

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