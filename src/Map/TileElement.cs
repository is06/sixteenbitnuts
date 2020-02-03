using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public enum TileType
    {
        Obstacle = 0,
        Traversable = 1,
        Platform = 2
    }

    public struct TileElement
    {
        public Size Size;
        public Vector2 Offset;
        public TileType Type;
    }
}
