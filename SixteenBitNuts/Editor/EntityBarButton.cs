using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts.Editor
{
    class EntityBarButton
    {
        #region Constants

        public const int BUTTON_SIZE = 20;

        #endregion

        #region Fields

        private Vector2 position;

        #endregion

        #region Properties

        public int Id { get; private set; }
        public bool IsSelected { get; set; }
        public EntityBar EntityBar { get; private set; }
        public Rectangle HitBox
        {
            get
            {
                return new Rectangle(
                    (int)Math.Round(position.X) * (int)EntityBar.Editor.Map.Game.ScreenScale,
                    (int)Math.Round(position.Y) * (int)EntityBar.Editor.Map.Game.ScreenScale,
                    BUTTON_SIZE * (int)EntityBar.Editor.Map.Game.ScreenScale,
                    BUTTON_SIZE * (int)EntityBar.Editor.Map.Game.ScreenScale
                );
            }
        }

        #endregion

        #region Components

        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D buttonTexture;

        #endregion

        public EntityBarButton(EntityBar bar, int id, Vector2 position)
        {
            EntityBar = bar;
            Id = id;

            this.position = position;
            spriteBatch = new SpriteBatch(bar.Editor.Map.Graphics);
            buttonTexture = bar.Editor.Map.Content.Load<Texture2D>("editor/entity_button");
        }

        public void Update()
        {

        }

        public void Draw()
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(3));
            spriteBatch.Draw(
                buttonTexture,
                position,
                new Rectangle(0, 0, BUTTON_SIZE, BUTTON_SIZE),
                IsSelected ? Color.Lime : Color.White
            );
            spriteBatch.End();

            EntityBar.Editor.Map.CurrentMapSection.Tileset.Draw(
                position + new Vector2(2, 2),
                EntityBar.Editor.Map.CurrentMapSection.Tileset.GetSizeFromId(Id),
                EntityBar.Editor.Map.CurrentMapSection.Tileset.GetOffsetFromId(Id),
                0,
                Matrix.CreateScale(EntityBar.Editor.Map.Game.ScreenScale)
            );
        }
    }
}
