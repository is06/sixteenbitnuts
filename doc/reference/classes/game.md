# Game (class)

## Properties

### WindowTitle

Set this property in the constructor to change the window title.

Default value: **empty string**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        WindowTitle = "My Awesome Game";
    }
}
```

### WindowSize

Set this property in the constructor to set width and height of the window in pixels.

Default value: **Size(1280, 720)**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        WindowSize = new Size(1280, 720);
    }
}
```

### InternalSize

Set this property in the constructor to specify a different size in game. For a pixel art style game, a lower internal size than window size will make pixels bigger.

Default value: **WindowSize value**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        WindowSize = new Size(1280, 720);
        InternalSize = new Size(320, 180);
    }
}
```

### FrameRate

Set this property in the constructor to set how many frames per second should render.

Default value: **60**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        FrameRate = 30;
    }
}
```

### FullScreen

Set this property in the constructor to set if the game will run in fullscreen (true) or windowed (false)

Default value: **false**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        FullScreen = true;
    }
}
```

### TileSize

Set this property in the constructor to set the size of tiles in pixels.

Default value: **16**

```csharp
class MyGame : SixteenBitNuts.Game
{
    public MyGame()
    {
        TileSize = 8;
    }
}
```

## Methods

Coming soon...
