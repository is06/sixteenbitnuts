﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts.Editor
{
    public enum ToolbarButtonType
    {
        Tile,
        Entity,
        Selection,
    }

    public abstract class ToolbarButton
    {
        public const int BUTTON_SIZE = 60;

        public ToolbarButtonType Type { get; set; }
        public Vector2 Position { get; set; }
        public int Id { get; set; }
        public Tileset? Tileset { get; set; }
        public string? GroupName { get; set; }
        public bool IsSelected { get; set; }
        public Toolbar Toolbar { get; private set; }
        public Rectangle HitBox
        {
            get
            {
                return new Rectangle(
                    (int)Math.Round(Position.X),
                    (int)Math.Round(Position.Y),
                    BUTTON_SIZE,
                    BUTTON_SIZE
                );
            }
        }

        private readonly Texture2D buttonTexture;

        public ToolbarButton(Toolbar toolbar)
        {
            Toolbar = toolbar;
            buttonTexture = toolbar.Editor.Map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/entity_button");
        }

        public virtual void Draw()
        {
            Toolbar.Editor.Map.Game.SpriteBatch?.Begin(samplerState: SamplerState.PointClamp);

            Toolbar.Editor.Map.Game.SpriteBatch?.Draw(
                buttonTexture,
                Position,
                new Rectangle(0, 0, 20, 20),
                IsSelected ? Color.Lime : Color.White,
                0,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                0
            );

            Toolbar.Editor.Map.Game.SpriteBatch?.End();
        }
    }
}
