using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Entity : MapElement
    {
        protected readonly Map map;

        public Entity(Map map, SpriteBatch spriteBatch) : base(map.Game.GraphicsDevice, spriteBatch)
        {
            this.map = map;

            DebugColor = Color.Orange;
        }

        public override void DebugDraw()
        {
            base.DebugDraw();
        }
    }
}
