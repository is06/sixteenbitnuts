using Microsoft.Xna.Framework;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public class Tile : MapElement, ITile
    {
        public int Id { get; private set; }

        private readonly Tileset tileset;

        public Tile(Map map, Tileset tileset, int id, Vector2 position, Vector2 size, TileType type) : base(map)
        {
            this.tileset = tileset;

            // Properties
            Id = id;
            Position = position;
            Size = size;
            IsObstacle = type == TileType.Obstacle;
        }

        public override void Draw()
        {
            tileset.Draw(Position, Size, tileset.GetOffsetFromId(Id), Vector2.One);
        }

        public override void DebugDraw()
        {
            tileset.DebugDraw(Position.ToPoint(), Size.ToPoint(), DebugColor);
        }
    }
}
