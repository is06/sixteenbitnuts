using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class DebugHitBox
    {
        private readonly Box graphicBox;

        public DebugHitBox(Game game, BoundingBox hitBox, int thickness, Color color)
        {
            graphicBox = new Box(
                game,
                new Rectangle(
                    new Point((int)hitBox.Min.X, (int)hitBox.Min.Y),
                    new Point((int)(hitBox.Max.X - hitBox.Min.X), (int)(hitBox.Max.Y - hitBox.Min.Y))
                ),
                thickness,
                color
            );
        }

        public void Draw()
        {
            graphicBox.Draw();
        }
    }
}
