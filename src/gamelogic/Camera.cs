using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    public class Camera
    {
        public Vector2 Position;

        public Matrix Transform
        {
            get
            {
                Vector2 viewPortCenter = new Vector2(map.ScrollAreaSize.X / 2, map.ScrollAreaSize.Y / 2);
                Vector2 translation = -Position + viewPortCenter;
                return Matrix.CreateTranslation((float)Math.Round(translation.X), (float)Math.Round(translation.Y), 0);
            }
        }

        private readonly Map map;

        public Camera(Map map)
        {
            this.map = map;
        }

        public void Update()
        {
            int hcenter = map.ScrollAreaSize.X / 2;
            int vcenter = map.ScrollAreaSize.Y / 2;

            int left = map.CurrentSection.Bounds.X + hcenter;
            int right = map.CurrentSection.Bounds.X + map.CurrentSection.Bounds.Width - hcenter;
            int top = map.CurrentSection.Bounds.Y + vcenter;
            int bottom = map.CurrentSection.Bounds.Y + map.CurrentSection.Bounds.Height - vcenter;

            if (Position.X < left)
            {
                Position.X = left;
            }
            if (Position.X > right)
            {
                Position.X = right;
            }
            if (Position.Y < top)
            {
                Position.Y = top;
            }
            if (Position.Y > bottom)
            {
                Position.Y = bottom;
            }
        }
    }
}
