using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    class TransitionGuide
    {
        public Vector2 Position { get; set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Math.Round(Position.X) - 240, (int)Math.Round(Position.Y) - 135, 480, 270);
            }
        }
    }
}
