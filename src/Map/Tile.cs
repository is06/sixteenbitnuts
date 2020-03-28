using Microsoft.Xna.Framework;
using SixteenBitNuts.Interfaces;
using System;
using System.Runtime.Serialization;

namespace SixteenBitNuts
{
    [Serializable]
    public class Tile : MapElement, ITile, ISerializable
    {
        public int Id { get; private set; }
        public string MapTextDescription
        {
            get
            {
                return "ti " + Id +
                       " " + Position.X +
                       " " + Position.Y +
                       " 0";
            }
        }

        private readonly Tileset tileset;

        public Tile(Map map, Tileset tileset, int id, Vector2 position, Size size, TileType type) : base(map)
        {
            this.tileset = tileset;

            // Properties
            Id = id;
            Position = position;
            Size = size;
            IsObstacle = type == TileType.Obstacle;
        }

        public override void Draw(Matrix transform)
        {
            tileset.Draw(Position, Size.ToVector2(), tileset.GetOffsetFromId(Id), Vector2.One, transform);
        }

        public override void DebugDraw(Matrix transform)
        {
            tileset.DebugDraw(Position.ToPoint(), Size.ToPoint(), DebugColor, transform);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", Id);
            info.AddValue("x", Position.X);
            info.AddValue("y", Position.Y);
        }
    }
}
