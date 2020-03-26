using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Primitive
    {
        protected readonly Texture2D pixel;
        protected readonly Game game;

        public Primitive(Game game)
        {
            this.game = game;

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public virtual void Draw()
        {

        }
    }
}
