#!/bin/bash

# exit if any command fails
set -e

dotnet restore ./PusherServer.Core/project.json

# Instead, run directly with mono for the full .net version
dotnet build ./PusherServer.Core/project.json -c Release