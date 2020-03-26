using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    public class Circle : Primitive
    {
        public float Radius { get; set; }
        public Vector2 Position { get; set; }

        public Circle(Game game) : base(game)
        {

        }

        public override void Draw()
        {
            base.Draw();

            game.SpriteBatch?.Begin();

            int? oldX = null;
            int? oldY = null;

            for (double i = 0; i <= MathHelper.TwoPi; i += 0.005)
            {
                int x = (int)Math.Round(Radius * Math.Cos(i));
                int y = (int)Math.Round(Radius * Math.Sin(i));

                if (oldX != x && oldY != y)
                {
                    game.SpriteBatch?.Draw(pixel, Position + new Vector2(x, y), Color.White);
                }
            }

            game.SpriteBatch?.End();
        }
    }
}
