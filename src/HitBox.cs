using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public struct HitBox
    {
        public Vector2 Position { get; set; }
        public Size Size { get; set; }

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
                return Position.X + Size.Width;
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
                return Position.Y + Size.Height;
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
                return Size.Width;
            }
        }
        public float Height
        {
            get
            {
                return Size.Height;
            }
        }

        public HitBox(Vector2 position, Size size)
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
