using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Image
    {
        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D texture;

        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }

        public Image(Scene scene, Vector2 position, string filePath)
        {
            spriteBatch = new SpriteBatch(scene.Graphics);
            texture = scene.Content.Load<Texture2D>(filePath);

            Color = Color.White;
            Scale = Vector2.One;
            Position = position;
        }

        public void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                texture,
                Position,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color,
                0,
                new Vector2(0, 0),
                Scale,
                SpriteEffects.None,
                0
            );
            spriteBatch.End();
        }
    }
}
