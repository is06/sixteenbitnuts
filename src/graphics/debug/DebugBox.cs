using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    class DebugBox
    {
        private readonly Game game;
        private readonly Line[] lines;

        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        public DebugBox(Game game, Rectangle bounds, Color color)
        {
            this.game = game;

            lines = new Line[4] {
                new Line(),
                new Line(),
                new Line(),
                new Line()
            };

            Bounds = bounds;
            Color = color;
        }

        public void Update()
        {
            // Top
            lines[0].Origin = new Point(Bounds.Left, Bounds.Top + 1);
            lines[0].Destination = new Point(Bounds.Right, Bounds.Top + 1);
            lines[0].Color = Color;

            // Left
            lines[1].Origin = new Point(Bounds.Left + 1, Bounds.Top);
            lines[1].Destination = new Point(Bounds.Left + 1, Bounds.Bottom);
            lines[1].Color = Color;

            // Right
            lines[2].Origin = new Point(Bounds.Right, Bounds.Top);
            lines[2].Destination = new Point(Bounds.Right, Bounds.Bottom);
            lines[2].Color = Color;

            // Bottom
            lines[3].Origin = new Point(Bounds.Left, Bounds.Bottom);
            lines[3].Destination = new Point(Bounds.Right, Bounds.Bottom);
            lines[3].Color = Color;
        }

        public void Draw()
        {
            game.LineBatch?.Draw(lines);
        }
    }
}
