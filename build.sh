#!/bin/bash

# exit if any command fails
set -e

dotnet build -c Release

dotnet test ./PusherServer.Tests/PusherServer.Tests.csproj -c Release --no-build --no-restore
