Draw an image
=============

Asset management: Monogame content references
---------------------------------------------

Create a `Graphics.mgcb` file in `Content` directory:

```
#----------------------------- Global Properties ----------------------------#

/outputDir:bin/$(Platform)/Graphics
/intermediateDir:obj/$(Platform)/Graphics
/platform:Windows
/config:
/profile:Reach
/compress:False

#-------------------------------- References --------------------------------#

#---------------------------------- Content ---------------------------------#
```

This file handles all references to asset files (like sprite image files).

Then add a `ItemGroup` section with a `MonoGameContentReference` into the `csproj` file:

```xml
<ItemGroup>
    <MonoGameContentReference Include="Content/Graphics.mgcb" />
</ItemGroup>
```

Add the actual image asset
--------------------------

Edit the `Graphics.mgcb` file to add an image. Add a `#being` section with following lines:

```
#begin Graphics/myimage.png
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:Graphics/myimage.png
```

Then the asset file (here a PNG file) will be compiled into a XNB file when building the game and can be loaded by the MonoGame engine.

Use Image class
---------------

In 16-bit Nuts, the simplest way to draw an image is to use the `Image` class in a scene:

```csharp
using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace MyGame
{
    class MyScene : Scene
    {
        private readonly Image myImage;

        public MyScene(SixteenBitNuts.Game game) : base(game)
        {
            // We create the image here in the constructor
            // We specify position and asset path (file extension is not needed)
            myImage = new Image(this, new Vector(64, 64), "Graphics/myimage");
        }

        public override void Draw()
        {
            base.Draw();

            // Starting the sprite batch for this scene
            Game.SpriteBatch?.Begin();

            // Drawing the image
            myImage.Draw();

            // Ending the sprite batch
            Game.SpriteBatch?.End();
        }
    }
}
```