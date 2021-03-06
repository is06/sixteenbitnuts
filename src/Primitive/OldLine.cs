using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    class OldLine
    {
        private readonly Texture2D texture;
        private readonly Game game;

        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }

        public OldLine(Game game, Rectangle bounds, Color color)
        {
            this.game = game;
            texture = new Texture2D(game.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            Bounds = bounds;
            Color = color;
        }

        public void Draw(Matrix transform)
        {
            game.SpriteBatch?.Begin(transformMatrix: transform);

            game.SpriteBatch?.Draw(
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

            game.SpriteBatch?.End();
        }
    }
}
