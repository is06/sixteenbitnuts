﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class SpawnPoint : Entity
    {
        private readonly Texture2D debugTexture;

        public SpawnPoint(Map map, string name) : base(map, name)
        {
            IsVisible = false;
            debugTexture = LoadTexture("Engine/editor/spawn");
        }

        public override void Update(GameTime gameTime)
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
