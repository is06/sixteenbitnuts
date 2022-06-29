using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Camera
    {
        public Vector2 Position => position;

        private readonly Map map;
        private Vector2 position;

        public Camera(Map map)
        {
            this.map = map;
        }

        public void Update()
        {
            int hcenter = map.Game.InternalSize.X / 2;
            int vcenter = map.Game.InternalSize.X / 2;

            int left = map.CurrentSection.Bounds.X + hcenter;
            int right = map.CurrentSection.Bounds.X + map.CurrentSection.Bounds.Width - hcenter;
            int top = map.CurrentSection.Bounds.Y + vcenter;
            int bottom = map.CurrentSection.Bounds.Y + map.CurrentSection.Bounds.Height - vcenter;

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
}
