using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Tile : MapElement
    {
        #region Properties

        public int Id { get; }

        #endregion

        #region Components

        private readonly Tileset tileset;

        #endregion

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
