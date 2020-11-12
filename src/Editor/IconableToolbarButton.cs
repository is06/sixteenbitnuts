using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public abstract class IconableToolbarButton : ToolbarButton
    {
        protected Texture2D? iconTexture;

        public IconableToolbarButton(Toolbar toolbar) : base(toolbar)
        {

        }

        public override void Draw()
        {
            base.Draw();

            if (iconTexture != null)
            {
                Vector2 position = new Vector2(Position.X + 6, Position.Y + 6);

                Toolbar.Editor.Map.Game.SpriteBatch?.Begin(samplerState: SamplerState.PointClamp);

                float textureRatio = Size.Ratio(iconTexture.Width, iconTexture.Height);
                Vector2 origin = Vector2.Zero;

                if (textureRatio > 1.0f)
                {
                    origin.Y -= iconTexture.Height / 2f;
                }

                Toolbar.Editor.Map.Game.SpriteBatch?.Draw(
                    texture: iconTexture,
                    position: position,
                    sourceRectangle: new Rectangle(0, 0, iconTexture.Width, iconTexture.Height),
                    color: Color.White,
                    rotation: 0f,
                    origin: origin,
                    scale: new Vector2((3f / (iconTexture.Width / 16f * 3f) * 3f), (3f / (iconTexture.Height / 16f * 3f)) * 3f / textureRatio),
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );

                Toolbar.Editor.Map.Game.SpriteBatch?.End();
            }
        }
    }
}
