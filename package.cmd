%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe pusher-dotnet-server.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

if not exist Download\package\lib\net35 mkdir Download\package\lib\net35\

copy README.md Download\Package\lib\net35\

copy PusherServer\bin\Release\PusherServer.dll Download\Package\lib\net35\

copy PusherServer\bin\Release\PusherServer.xml Download\Package\lib\net35\

tools\nuget.exe update -self
tools\nuget.exe pack pusher-dotnet-server.nuspec -BasePath Download\Package -Output Download