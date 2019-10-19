using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Entity : MapElement
    {
        public Entity(Map map) : base(map)
        {
            DebugColor = Color.Orange;
        }

        public override void DebugDraw()
        {
            base.DebugDraw();
        }
    }
}
