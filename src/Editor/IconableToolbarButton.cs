using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts.Editor
{
    public abstract class IconableToolbarButton : ToolbarButton
    {
        protected Texture2D? iconTexture;

        public IToolbarIconable? LinkedMapElement;

        public IconableToolbarButton(Toolbar toolbar) : base(toolbar)
        {

        }

        public override void Draw()
        {
            base.Draw();

            if (iconTexture != null)
            {
                Toolbar.Editor.Map.Game.SpriteBatch?.Begin(samplerState: SamplerState.PointClamp);

                var textureOffset = new Point(0, 0);
                if (LinkedMapElement is IMapElement element)
                {
                    textureOffset = ((IToolbarIconable)element).ToolbarTextureOffset;
                }

                Toolbar.Editor.Map.Game.SpriteBatch?.Draw(
                    texture: iconTexture,
                    position: new Vector2(Position.X + 6, Position.Y + 6),
                    sourceRectangle: new Rectangle(textureOffset.X, textureOffset.Y, 16, 16),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 3f,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );

                Toolbar.Editor.Map.Game.SpriteBatch?.End();
            }
        }
    }
}
