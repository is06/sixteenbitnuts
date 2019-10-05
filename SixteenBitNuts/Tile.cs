using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Tile
    {
        #region Properties

        public int Id { get; }
        public Vector2 Position { get; set; }
        public BoundingBox HitBox
        {
            get
            {
                return new BoundingBox
                {
                    Min = new Vector3(Position.X, Position.Y, 0),
                    Max = new Vector3(Position.X + size.X, Position.Y + size.Y, 0)
                };
            }
        }

        #endregion

        #region Components

        private readonly Tileset tileset;
        private Vector2 size;
        private readonly float layer;
        public bool IsObstacle;

        #endregion

        public Tile(Tileset tileset, int id, Vector2 position, Vector2 size, TileType type, float layer)
        {
            // Properties
            Id = id;
            Position = position;

            this.tileset = tileset;
            this.size = size;
            this.layer = layer;

            IsObstacle = type == TileType.Obstacle;
        }

        public void Draw(Matrix transform)
        {
            tileset.Draw(Position, size, tileset.GetOffsetFromId(Id), layer, transform);
        }

        public void DebugDraw(Matrix transform)
        {
            tileset.DebugDraw(new Vector2(Position.X, Position.Y), transform);
        }
    }
}
