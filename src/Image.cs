using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Image
    {
        private readonly Texture2D texture;
        private readonly Scene scene;

        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }

        public Image(Scene scene, Vector2 position, string filePath)
        {
            this.scene = scene;
            texture = scene.Game.Content.Load<Texture2D>(filePath);

            Color = Color.White;
            Scale = Vector2.One;
            Position = position;
        }

        public void Draw()
        {
            scene.Game.SpriteBatch?.Draw(
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
        }
    }
}
