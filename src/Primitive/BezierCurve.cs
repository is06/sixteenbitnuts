using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class BezierCurve
    {
        private readonly Texture2D pixel;
        private readonly Game game;

        public Vector2 Origin { get; set; }
        public Vector2 CurvePoint { get; set; }
        public Vector2 Destination { get; set; }

        public BezierCurve(Game game)
        {
            this.game = game;
            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void Draw()
        {
            game.SpriteBatch?.Begin();

            for (float x = Origin.X; x <= Destination.X; x += 1.0f)
            {
                var y = Calc.Bezier2(Origin.Y, CurvePoint.Y, Destination.Y, x);

                game.SpriteBatch?.Draw(pixel, new Vector2(x, y), Color.White);
            }

            game.SpriteBatch?.End();
        }
    }
}
