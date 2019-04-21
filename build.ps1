dotnet clean
dotnet restore
dotnet test
dotnet publish -c Release -r win-x64 --self-contained false
dotnet publish -c Release -r osx-x64 --self-contained false
dotnet publish -c Release -r linux-x64 --self-contained false

New-Item -Name dist -ItemType "directory"
$version = Get-Content -Path VERSION

Compress-Archive -Path ./source/GraylesGui/bin/Release/netcoreapp2.2/win-x64/publish/* -DestinationPath dist/GraylesGui-win-$version.zip
Compress-Archive -Path ./source/GraylesGui/bin/Release/netcoreapp2.2/linux-x64/publish/* -DestinationPath dist/GraylesGui-linux-$version.zip
Compress-Archive -Path ./source/GraylesGui/bin/Release/netcoreapp2.2/osx-x64/publish/* -DestinationPath dist/GraylesGui-osx-$version.zip