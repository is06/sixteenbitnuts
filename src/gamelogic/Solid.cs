using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Solid
    {
        public Rectangle Bounds { get; set; }

        private readonly DebugBox debugBox;

        public Solid(Game game, Rectangle bounds)
        {
            Bounds = bounds;

            debugBox = new DebugBox(game, Bounds, Color.Red);
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
