using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class DebugHitBox
    {
        private readonly Box graphicBox;

        public Color Color
        {
            get
            {
                return graphicBox.Color;
            }
            set
            {
                graphicBox.Color = value;
            }
        }

        public DebugHitBox(Game game, int thickness, Color color)
        {
            graphicBox = new Box(
                game,
                new Rectangle(0, 0, 0, 0),
                thickness,
                color
            );
        }

        public void Update(BoundingBox hitBox)
        {
            graphicBox.Bounds = new Rectangle(
                new Point((int)hitBox.Min.X, (int)hitBox.Min.Y),
                new Point((int)(hitBox.Max.X - hitBox.Min.X), (int)(hitBox.Max.Y - hitBox.Min.Y))
            );
            graphicBox.Update();
        }

        public void Draw()
        {
            graphicBox.Draw();
        }
    }
}
