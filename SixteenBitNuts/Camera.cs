using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SixteenBitNuts
{
    public class Camera
    {
        #region Fields

        private Vector2 position;
        private Viewport viewPort;
        private readonly Map map;

        #endregion

        #region Properties

        public bool IsMovingToNextSection { get; set; }
        public bool CanOverrideLimits { get; set; }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Vector2 CornerPosition
        {
            get
            {
                return new Vector2(position.X - viewPort.Width / 2, position.Y - viewPort.Height / 2);
            }
        }
        public Matrix Transform
        {
            get
            {
                Vector2 viewPortCenter = new Vector2(viewPort.Width / 2, viewPort.Height / 2);
                Vector2 translation = -position + viewPortCenter;
                return Matrix.CreateTranslation((float)Math.Round(translation.X), (float)Math.Round(translation.Y), 0);
            }
        }
        public Viewport ViewPort
        {
            get
            {
                return viewPort;
            }
        }

        #endregion

        public Camera(Map map, Vector2 position, Viewport viewPort)
        {
            // Fields
            this.map = map;
            this.position = position;
            this.viewPort = viewPort;
        }

        public void Update(GameTime gameTime)
        {
            if (!CanOverrideLimits)
            {
                int left = map.CurrentMapSection.Bounds.X + viewPort.Width / 2;
                int right = map.CurrentMapSection.Bounds.X + map.CurrentMapSection.Bounds.Width - viewPort.Width / 2;
                int top = map.CurrentMapSection.Bounds.Y + viewPort.Height / 2;
                int bottom = map.CurrentMapSection.Bounds.Y + map.CurrentMapSection.Bounds.Height - viewPort.Height / 2;

                if (position.X < left)
                {
                    position.X = left;
                }
                if (position.X > right)
                {
                    position.X = right;
                }
                if (position.Y < top)
                {
                    position.Y = top;
                }
                if (position.Y > bottom)
                {
                    position.Y = bottom;
                }
            }
        }

        public void MoveLeft(float value)
        {
            position.X -= value;
        }

        public void MoveRight(float value)
        {
            position.X += value;
        }

        public void MoveUp(float value)
        {
            position.Y -= value;
        }

        public void MoveDown(float value)
        {
            position.Y += value;
        }
    }
}
