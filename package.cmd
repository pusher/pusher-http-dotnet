"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\pusher-dotnet-server.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
dotnet build .\PusherServer.Core

tools\nuget.exe update -self
tools\nuget.exe pack .\PusherServer.nuspec