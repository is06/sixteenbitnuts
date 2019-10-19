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

        private readonly SpriteFont font;
        private readonly Scene scene;

        public Label(Scene scene)
        {
            this.scene = scene;
            font = scene.Game.Content.Load<SpriteFont>("Engine/fonts/numbers");
        }

        public void Update()
        {

        }

        public void Draw()
        {
            if (IsVisible)
            {
                scene.Game.SpriteBatch.DrawString(font, Text, Position, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
