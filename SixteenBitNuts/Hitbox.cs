using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public struct HitBox
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public float Left
        {
            get
            {
                return Position.X;
            }
        }
        public float Right
        {
            get
            {
                return Position.X + Size.X;
            }
        }
        public float Top
        {
            get
            {
                return Position.Y;
            }
        }
        public float Bottom
        {
            get
            {
                return Position.Y + Size.Y;
            }
        }
        public float X
        {
            get
            {
                return Position.X;
            }
        }
        public float Y
        {
            get
            {
                return Position.Y;
            }
        }
        public float Width
        {
            get
            {
                return Size.X;
            }
        }
        public float Height
        {
            get
            {
                return Size.Y;
            }
        }

        public HitBox(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public bool Intersects(HitBox other)
        {
            if (Left < other.Right && Right > other.Left && Top <= other.Bottom && Bottom >= other.Top)
            {
                return true;
            }
            return false;
        }
    }
}
