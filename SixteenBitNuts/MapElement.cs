using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class MapElement
    {
        public bool IsObstacle { get; set; }
        public bool IsPlatform { get; set; }
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
    }
}
