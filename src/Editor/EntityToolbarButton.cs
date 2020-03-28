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

            if (type == "spawn")
            {
                texture = bar.Editor.Map.Game.Content.Load<Texture2D>("Engine/editor/" + type);
            }
            else
            {
                texture = bar.Editor.Map.Game.Content.Load<Texture2D>("Game/sprites/entities/" + type);
            }
        }

        public override void Draw()
        {
            base.Draw();

            Vector2 position = new Vector2(
                (Position.X + 6),
                (Position.Y + 6)
            );

            Toolbar.Editor.Map.Game.SpriteBatch?.Begin();

            Toolbar.Editor.Map.Game.SpriteBatch?.Draw(
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

            Toolbar.Editor.Map.Game.SpriteBatch?.End();
        }
    }
}
