using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    class Label
    {
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public bool IsVisible { get; set; }

        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;

        public Label(Map map)
        {
            spriteBatch = new SpriteBatch(map.Graphics);
            font = map.Content.Load<SpriteFont>("fonts/numbers");
        }

        public void Update()
        {

        }

        public void Draw(Matrix transform)
        {
            if (IsVisible)
            {
                spriteBatch.Begin(transformMatrix: transform);
                spriteBatch.DrawString(font, Text, Position, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.End();
            }
        }
    }
}
