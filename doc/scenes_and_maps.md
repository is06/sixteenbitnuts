Scenes and maps
===============

A game in this engine is made of scenes. You can create a scene for your levels, for the title screen, menus...

Simple scene
------------

Let's create a very simple scene that does nothing (for now):

```csharp
using SixteenBitNuts;

namespace MyGame
{
    class MyScene : Scene
    {
    }
}
```

This is as simple as that, you create a class of type `Scene`.

Scenes have an `Update` and a `Draw` method that you can override to handle all events and displays:

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class MyScene : Scene
    {
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

Maps
----

Maps are a type of scene provided by the engine. Maps contains a collection of **sections** and a **player**.

It is recommended to create a custom class of type `Map` to handle events in it more easily (like collisions):

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class Level : Map
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Events of levels
        }
    }
}
```

Overriding the `Draw` method in Maps are not need at all because the engine already manages how to perform the display of every elements in the map.