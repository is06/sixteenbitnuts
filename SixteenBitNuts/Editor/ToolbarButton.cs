using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts.Editor
{
    public class ToolbarButton
    {
        #region Constants

        public const int BUTTON_SIZE = 20;

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
                    (int)Math.Round(Position.X) * (int)Toolbar.Editor.Map.Game.ScreenScale,
                    (int)Math.Round(Position.Y) * (int)Toolbar.Editor.Map.Game.ScreenScale,
                    BUTTON_SIZE * (int)Toolbar.Editor.Map.Game.ScreenScale,
                    BUTTON_SIZE * (int)Toolbar.Editor.Map.Game.ScreenScale
                );
            }
        }

        #endregion

        #region Components

        protected readonly SpriteBatch spriteBatch;
        private readonly Texture2D buttonTexture;

        #endregion

        public ToolbarButton(Toolbar bar)
        {
            Toolbar = bar;

            spriteBatch = new SpriteBatch(bar.Editor.Map.Graphics);
            buttonTexture = bar.Editor.Map.Content.Load<Texture2D>("Engine/editor/entity_button");
        }

        public virtual void Draw()
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(3));
            spriteBatch.Draw(
                buttonTexture,
                Position,
                new Rectangle(0, 0, BUTTON_SIZE, BUTTON_SIZE),
                IsSelected ? Color.Lime : Color.White
            );
            spriteBatch.End();
        }
    }
}
