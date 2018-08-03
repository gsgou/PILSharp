# PILSharp

PILSharp gives Xamarin developers high-performance image processing cross-platform APIs for their mobile applications.

## Installation
* NuGet Official Releases: [![NuGet](https://img.shields.io/nuget/v/PILSharp.svg?label=NuGet)](https://www.nuget.org/packages/PILSharp)

## Supported Platforms
PILSharp is focused on the following platforms:
- iOS (10+)
- Android (4.4+)

## Current APIs:
The following cross-platform APIs are available in PILSharp:
- [x] Crop (Remove border from image.)
- [x] Equalize (Equalize the image histogram.)
- [x] Expand (Add border to the image.)
- [x] Fit (Returns a sized version of the image.)

## Building PILSharp
PILSharp is built with the new SDK style projects with multi-targeting enabled. 

If building on Visual Studio 2017 you will need the following SDKs and workloads installed:

### Workloads need:
- Xamarin
- .NET Core

### You will need the following SDKs
- Android 8.1 SDK Installed

If using Visual Studio for Mac the project can be built at the command line with MSBuild. To build through the command line, navigate to where PILSharp.csproj exists then run:

```csharp
dotnet restore
msbuild PILSharp.csproj
```
## License
Distributed with the MIT license.
