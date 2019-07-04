
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\amd64\msbuild.exe" .\pusher-dotnet-everything-server.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

tools\nuget.exe update -self
tools\nuget.exe pack .\PusherServer\PusherServer.csproj -verbosity detailed -properties Configuration=Release