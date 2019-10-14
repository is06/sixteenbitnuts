using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Tile : MapElement
    {
        #region Properties

        public int Id { get; }

        #endregion

        #region Components

        private readonly Tileset tileset;
        private readonly float layer;

        #endregion

        public Tile(Tileset tileset, int id, Vector2 position, Vector2 size, TileType type, float layer) : base(tileset.Graphics)
        {
            // Properties
            Id = id;
            Position = position;
            Size = size;

            this.tileset = tileset;
            this.layer = layer;

            IsObstacle = type == TileType.Obstacle;
        }

        public override void Draw(Matrix transform)
        {
            tileset.Draw(Position, Size, tileset.GetOffsetFromId(Id), layer, transform);
        }

        public override void DebugDraw(Matrix transform)
        {
            tileset.DebugDraw(Position.ToPoint(), Size.ToPoint(), transform);
        }
    }
}
