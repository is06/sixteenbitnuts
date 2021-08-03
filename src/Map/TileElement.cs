using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public enum TileType
    {
        Obstacle = 0,
        Traversable = 1,
        Platform = 2
    }

    public enum TileLayer
    {
        Foreground = 0,
        Background = 1
    }

    public struct TileElement
    {
        public Rectangle Bounds;
        public TileType Type;
        public TileLayer Layer;
    }
}
