using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public abstract class Label
    {
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public bool IsVisible { get; set; }

        protected SpriteFont? font;
        protected readonly Scene scene;

        public Label(Scene scene)
        {
            this.scene = scene;
            Text = "";
            Color = Color.White;
        }

        public virtual void Draw(Matrix transform)
        {
            if (IsVisible && font != null)
            {
                scene.Game.SpriteBatch?.Begin(transformMatrix: transform);

                scene.Game.SpriteBatch?.DrawString(font, Text, Position, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                scene.Game.SpriteBatch?.End();
            }
        }
    }
}
