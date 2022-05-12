using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    class Collider
    {
        public Rectangle Bounds;

        private readonly DebugBox debugBox;

        public Collider(Game game, Point size)
        {
            Bounds = new Rectangle(Point.Zero, size);

            debugBox = new DebugBox(game, Bounds, Color.Lime);
        }

        public void Update()
        {
            debugBox.Bounds = Bounds;
            debugBox.Update();
        }

        public void DebugDraw()
        {
            debugBox.Draw();
        }
    }
}
