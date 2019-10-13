using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Tile
    {
        #region Properties

        public int Id { get; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public BoundingBox HitBox
        {
            get
            {
                return new BoundingBox
                {
                    Min = new Vector3(Position.X, Position.Y, 0),
                    Max = new Vector3(Position.X + Size.X, Position.Y + Size.Y, 0)
                };
            }
        }

        #endregion

        #region Components

        private readonly Tileset tileset;
        private readonly float layer;
        public bool IsObstacle;

        #endregion

        public Tile(Tileset tileset, int id, Vector2 position, Vector2 size, TileType type, float layer)
        {
            // Properties
            Id = id;
            Position = position;
            Size = size;

            this.tileset = tileset;
            this.layer = layer;

            IsObstacle = type == TileType.Obstacle;
        }

        public void Draw(Matrix transform)
        {
            tileset.Draw(Position, Size, tileset.GetOffsetFromId(Id), layer, transform);
        }

        public void DebugDraw(Matrix transform)
        {
            tileset.DebugDraw(Position.ToPoint(), Size.ToPoint(), transform);
        }
    }
}
