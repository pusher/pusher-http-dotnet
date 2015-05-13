%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe pusher-dotnet-server.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

tools\nuget.exe update -self
tools\nuget.exe pack PusherServer\PusherServer.csproj -Prop Configuration=Release