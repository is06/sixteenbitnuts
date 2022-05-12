using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    /// <summary>
    /// Represents a portion of a tileset which can be drawn on the map as a tile instance.
    /// </summary>
    public struct TilesetFragment
    {
        public Point Size;
        public Point Position;
        public TileType Type;
    }
}
