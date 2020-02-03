﻿using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Box
    {
        private Line[] lines;

        public Rectangle Bounds { get; set; }
        public int Thickness { get; set; }
        public Color Color { get; set; }

        public Box(Game game, Rectangle bounds, int thickness, Color color)
        {
            Bounds = bounds;
            Thickness = thickness;
            Color = color;

            InitLines(game, color);
        }

        protected virtual void InitLines(Game game, Color color)
        {
            lines = new Line[4];
            lines[0] = new Line(game, new Rectangle(0, 0, 1, 1), color);
            lines[1] = new Line(game, new Rectangle(0, 0, 1, 1), color);
            lines[2] = new Line(game, new Rectangle(0, 0, 1, 1), color);
            lines[3] = new Line(game, new Rectangle(0, 0, 1, 1), color);
        }

        public void Update()
        {
            foreach (Line line in lines)
            {
                line.Color = Color;
            }

            // top
            lines[0].Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Thickness);
            // bottom
            lines[1].Bounds = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - Thickness, Bounds.Width, Thickness);
            // left
            lines[2].Bounds = new Rectangle(Bounds.X, Bounds.Y, Thickness, Bounds.Height);
            // right
            lines[3].Bounds = new Rectangle(Bounds.X + Bounds.Width - Thickness, Bounds.Y, Thickness, Bounds.Height);
        }

        public void Draw()
        {
            foreach (Line line in lines)
            {
                line.Draw();
            }
        }
    }
}