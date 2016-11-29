#!/usr/bin/env bash

dotnet restore
dotnet build ./PusherServer.Core -c Release
xbuild ./PusherServer/PusherServer.csproj
xbuild ./PusherServer.Tests/PusherServer.Tests.csproj
nunit-console PusherServer.Tests/bin/Release/PusherServer.Tests.dll