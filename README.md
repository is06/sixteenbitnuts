# 16-bit Nuts

Game engine to build 16-bit styled platformers games.

## Getting started

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
        <PackageReference Include="is06.SixteenBitNuts.Framework" Version="0.3.1" />
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

## Build

```bash
dotnet run
```

## All documentation pages

[16-bit nuts documentation](doc/)
