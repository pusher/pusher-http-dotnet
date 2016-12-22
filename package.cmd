
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\pusher-dotnet-everything-server.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

tools\nuget.exe update -self
tools\nuget.exe pack .\PusherServer\PusherServer.csproj -verbosity detailed -properties Configuration=Release