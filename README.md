16-bit Nuts
===========

Game engine to build 16-bit styled platformers games.

Getting started
---------------

16-bit Nuts uses FMOD Studio API. Create a runtimes directory with the following structure:

- runtimes
  - linux-x64
    - native
  - osx
    - native
  - win-x64
    - native
  - win-x86
    - native

And place every fmod and fmodstudio dynamic libraries into every corresponding native directories.

Create a `*.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>netcoreapp3.1</TargetFramework>
        <Product>MyGame</Product>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.0.1641" />
        <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.0.1641" />
        <Reference Include="SixteenBitNuts">
            <HintPath>path/to/sixteenbitnuts/bin/$(Configuration)/netstandard2.0/SixteenBitNuts.dll</HintPath>
        </Reference>
        <None Include="runtimes/win-x86/native/fmod.dll" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/win-x86/native/fmodstudio.dll" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/win-x64/native/fmod.dll" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/win-x64/native/fmodstudio.dll" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/linux-x64/native/libfmod.so.11.8" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Linux)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/linux-x64/native/libfmodstudio.so.11.8" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Linux)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/osx/native/libfmod.dylib" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::OSX)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
        <None Include="runtimes/osx/native/libfmodstudio.dylib" Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::OSX)))' == 'true'">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
    </ItemGroup>
</Project>
```

Create a `MyGame.cs` file (the name can be different):

```csharp
using SixteenBitNuts;

namespace MyGame
{
    class MyGame : Game
    {
        public MyGame()
        {
            WindowTitle = "MyGame";
            WindowSize = new Size(640, 480);
            InternalSize = new Size(320, 240);
            FrameRate = 60;
        }
    }
}
```

Create a `Program.cs` file with main function that uses the game class:

```csharp
using System;

namespace MyGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var game = new MyGame();
            game.Run();  
        }
    }
}
```

Build
-----

```bash
dotnet run
```

A warning related to 'MonoGameContentReference' can be raised but it's temporary. You can fix this by creating a content reference (see documentation about assets).
