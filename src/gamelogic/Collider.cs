using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    class Collider
    {
        public Rectangle Bounds;

        public Collider(Point size)
        {
            Bounds = new Rectangle(Point.Zero, size);
        }
    }
}
