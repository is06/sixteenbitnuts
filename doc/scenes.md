# Scenes

A game in this engine is made of scenes. You can create a scene for your levels, for the title screen, menus...

In this section of the documentation, we will create a title screen.

## Creation

Let's create a very simple scene that will represents our Title Screen:

**TitleScreen.cs**

```csharp
using SixteenBitNuts;

namespace MyGame
{
    class TitleScreen : Scene
    {
        public TitleScreen(Game game) : base(game)
        {

        }
    }
}
```

This is as simple as that, you create a class of type `Scene`.

Scenes have an `Update` and a `Draw` method that you can override to handle all events and displays:

**TitleScreen.cs**

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class TitleScreen : Scene
    {
        public TitleScreen(Game game) : base(game)
        {
            // Here scene initialization
        }

        public override void Update(GameTime gameTime)
        {
            // Here handle events
        }

        public override void Draw()
        {
            // Here perform all displays
        }
    }
}
```

Tip: place your scene classes in a dedicated directory (like `Scenes`) of your project so you can easily find them.

## Loading on start

If you want to load your scene on game start, you have to do it from the game class.

In the `LoadContent` method you have to override, you can replace the `currentScene` reference by a new one of your TitleScreen.

**MyGame.cs**

```csharp
namespace MyGame
{
    class MyGame : Game
    {
        ...

        protected override void LoadContent()
        {
            base.LoadContent();

            currentScene = new TitleScreen(this);
        }
    }
}
```

As `LoadContent` method is called after the creation of the game, it acts like the `currentScene` is replaced just at the beginning. So the TitleScreen is loaded on start.

But it's hard to see if it works because the window is still black as our scene does nothing. Let's just add a console line write in the scene to be sure it's working:

**Scenes/TitleScreen.cs**

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class TitleScreen : Scene
    {
        public TitleScreen(Game game) : base(game)
        {
            System.Console.WriteLine("On Title Screen");
        }

        ...
    }
}
```

So now when you run the command `dotnet run`, you will see the message in the terminal.

---

Next: Draw an image (soon)
