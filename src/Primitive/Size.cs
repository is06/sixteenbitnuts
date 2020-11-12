using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public struct Size
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float AspectRatio
        {
            get
            {
                return Ratio(Width, Height);
            }
        }

        public Point ToPoint()
        {
            return new Point((int)Width, (int)Height);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }

        public override string ToString()
        {
            return Width + " " + Height;
        }

        public static float Ratio(float width, float height)
        {
            return width / height;
        }
    }
}
