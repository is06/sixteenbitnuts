using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts.Editor
{
    public class ToolbarButton
    {
        #region Constants

        public const int BUTTON_SIZE = 60;

        #endregion

        #region Properties

        public Vector2 Position { get; set; }
        public int Id { get; set; }
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

        #endregion

        #region Components

        private readonly Texture2D buttonTexture;

        #endregion

        public ToolbarButton(Toolbar bar)
        {
            Toolbar = bar;
            buttonTexture = bar.Editor.Map.Game.Content.Load<Texture2D>("Engine/editor/entity_button");
        }

        public virtual void Draw()
        {
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
        }
    }
}
