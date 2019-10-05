using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    class SpawnPoint : Entity
    {
        private readonly Texture2D debugTexture;

        public SpawnPoint(Map map) : base(map)
        {
            debugTexture = map.Content.Load<Texture2D>("editor/spawn");
        }

        public override void Update()
        {

        }

        public override void Draw(Matrix transform)
        {
            base.Draw(transform);
        }

        public override void DebugDraw(Matrix transform)
        {
            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(
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
            spriteBatch.End();

            base.DebugDraw(transform);
        }
    }
}
