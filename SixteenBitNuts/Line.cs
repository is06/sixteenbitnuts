using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    class Line
    {
        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D texture;

        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        public Line(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            this.spriteBatch = spriteBatch;
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            Bounds = bounds;
            Color = color;
        }

        public void Draw()
        {
            spriteBatch.Draw(
                texture,
                new Vector2(Bounds.X, Bounds.Y),
                new Rectangle(0, 0, 1, 1),
                Color,
                0,
                new Vector2(0, 0),
                new Vector2(Bounds.Width, Bounds.Height),
                SpriteEffects.None,
                0
            );
        }
    }
}
