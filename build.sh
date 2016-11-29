#!/usr/bin/env bash

dotnet restore
dotnet build ./PusherServer.Core -c Release
xbuild ./PusherServer/PusherServer.csproj