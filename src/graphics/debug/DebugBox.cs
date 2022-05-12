using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    class DebugBox
    {
        private readonly DebugQuad[] lines;

        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        public DebugBox(Game game, Rectangle bounds, Color color)
        {
            lines = new DebugQuad[4];
            lines[0] = new DebugQuad(game, Rectangle.Empty, color);
            lines[1] = new DebugQuad(game, Rectangle.Empty, color);
            lines[2] = new DebugQuad(game, Rectangle.Empty, color);
            lines[3] = new DebugQuad(game, Rectangle.Empty, color);

            Bounds = bounds;
            Color = color;
        }

        public void Update()
        {
            foreach (var line in lines)
            {
                line.Color = Color;
            }

            lines[0].Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1);
            lines[1].Bounds = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.Width, 1);
            lines[2].Bounds = new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height);
            lines[3].Bounds = new Rectangle(Bounds.X + Bounds.Width - 1, Bounds.Y, 1, Bounds.Height);
        }

        public void Draw()
        {
            foreach (var line in lines)
            {
                line.Draw();
            }
        }
    }
}
