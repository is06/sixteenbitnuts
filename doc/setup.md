# Setup

Here are the first tasks we have to do to get a working simple program.

## Create a project

This project file contains all the information to actually build the game.
We will start with a very simple project file containing just the minimum requirements:

**MyGame.csproj**

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

## Game class

Now we can create a game class that will represent the game itself. The class has to be of type `Game` from the `SixteenBitNuts` namespace.

**MyGame.cs**

```csharp
using SixteenBitNuts;

namespace MyGame
{
    class MyGame : Game
    {
        public MyGame()
        {
            // Any initialization here
        }
    }
}
```

To know more about customizing the game class, have a look at [this page](game_class.md)

## Program entry point

The program entry point is a class containing the entry point method. The class has to contain this static method called `Main`.

Inside the method, we will just instanciate the game class previously created and call the `Run` method.

**Program.cs**

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

## Get the engine graphics

In order to use the design tools provided with the engine, we need to get all graphic assets of the engine from a independent repository.

Run this command in a terminal to create a submodule in your project that will contains all assets:

```shell
git submodule add https://gitlab.com/is06team/sixteenbitnuts.enginegraphics.git ./EngineGraphics
```

### MonoGame content builder

This package will compile the 16-bit nuts engine graphic assets when we will build the game. We have to add it to the dependencies ItemGroup in the project file:

```xml
<ItemGroup>
    <!-- other dependencies... -->
    <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.0.1641" />
</ItemGroup>
```

### Reference of engine graphics

Then add this reference to the graphics library in a new ItemGroup in the project file:

```xml
<ItemGroup>
    <MonoGameContentReference Include="EngineGraphics\EngineGraphics.mgcb" />
</ItemGroup>
```

## Run the program

We just need to run this command in a terminal to build and run the game:

```shell
dotnet run
```
