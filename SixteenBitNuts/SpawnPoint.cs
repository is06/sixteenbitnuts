using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class SpawnPoint : Entity
    {
        public string Name { get; private set; }

        private readonly Texture2D debugTexture;

        public SpawnPoint(Map map, string name) : base(map)
        {
            Name = name;
            debugTexture = map.Game.Content.Load<Texture2D>("Engine/editor/spawn");
        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void EditorDraw()
        {
            DebugDraw();

            base.EditorDraw();
        }

        public override void DebugDraw()
        {
            map.Game.SpriteBatch.Draw(
                texture: debugTexture,
                position: new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y)),
                sourceRectangle: new Rectangle(0, 0, 16, 16),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, 0),
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );

            base.DebugDraw();
        }
    }
}
