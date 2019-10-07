using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public class EntityToolbarButton : ToolbarButton
    {
        private readonly Texture2D texture;

        public string Type { get; private set; }

        public EntityToolbarButton(Toolbar bar, string type) : base(bar)
        {
            Type = type;

            texture = bar.Editor.Map.Content.Load<Texture2D>("Game/sprites/entities/" + type);
        }

        public override void Draw()
        {
            base.Draw();

            Vector2 position = new Vector2(
                (Position.X + 2) * 3,
                (Position.Y + 2) * 3
            );

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                texture: texture,
                position: position,
                sourceRectangle: new Rectangle(0, 0, 16, 16),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 3f,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
            spriteBatch.End();
        }
    }
}
