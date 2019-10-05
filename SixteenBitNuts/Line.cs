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

        public Line(Map map, Rectangle bounds, Color color)
        {
            spriteBatch = new SpriteBatch(map.Graphics);
            texture = map.Content.Load<Texture2D>("sprites/pixel");
            Bounds = bounds;
            Color = color;
        }

        public void Draw(Matrix transform)
        {
            spriteBatch.Begin(transformMatrix: transform);
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
            spriteBatch.End();
        }
    }
}
