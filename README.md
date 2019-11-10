16-bit Nuts
===========

Game engine to build 16-bit styled platformers games.

Empty game project
------------------

- Create a Monogame solution
- Create a class file with these:

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class MyGame : SixteenBitNuts.Game
    {
        public MyGame()
        {
            Window.Title = "My Game";
            WindowSize = new Size(1280, 720);
            InternalSize = new Size(320, 180);
            FrameRate = 60;
        }

        protected override void LoadContent()
        {
            currentScene = new Map(this, "test");
        }
    }
}
```
