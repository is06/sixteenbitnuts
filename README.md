16-bit Nuts
===========

Game engine to build 16-bit styled platformers games.

Getting started
---------------

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
        <PackageReference Include="is06.SixteenBitNuts.Framework" Version="0.2.0" />
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
