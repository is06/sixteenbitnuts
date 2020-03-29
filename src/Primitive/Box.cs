using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Box
    {
        private readonly OldLine[] lines;

        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        public Box(Game game, Rectangle bounds, Color color)
        {
            Bounds = bounds;
            Color = color;

            lines = new OldLine[4];
            lines[0] = new OldLine(game, new Rectangle(0, 0, 1, 1), color);
            lines[1] = new OldLine(game, new Rectangle(0, 0, 1, 1), color);
            lines[2] = new OldLine(game, new Rectangle(0, 0, 1, 1), color);
            lines[3] = new OldLine(game, new Rectangle(0, 0, 1, 1), color);
        }

        public void Update()
        {
            foreach (OldLine line in lines)
            {
                line.Color = Color;
            }

            // top
            lines[0].Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1);
            // bottom
            lines[1].Bounds = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.Width, 1);
            // left
            lines[2].Bounds = new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height);
            // right
            lines[3].Bounds = new Rectangle(Bounds.X + Bounds.Width - 1, Bounds.Y, 1, Bounds.Height);
        }

        public void Draw(Matrix transform)
        {
            foreach (OldLine line in lines)
            {
                line.Draw(transform);
            }
        }
    }
}
