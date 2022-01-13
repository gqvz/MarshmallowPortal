# MarshmallowPortal

## Compiling

### Client

#### OSX
`dotnet publish -c Release -p:PublishSingleFile=true -p:RuntimeIdentifier=osx.10.12-x64 -p:IncludeNativeLibrariesForSelfExtract=true`
The output should be in MarshmallowPortal/MarshmallowPortal.Client/bin/Release/net6.0/osx.10.12-x64/publish

#### Windows
`dotnet publish -c Release -p:PublishSingleFile=true -p:RuntimeIdentifier=win10-x64 -p:IncludeNativeLibrariesForSelfExtract=true`
The output should be in MarshmallowPortal\MarshmallowPortal.Client\bin\Release\net6.0\win10-x64\publish
