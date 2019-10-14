using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Entity : MapElement
    {
        protected readonly Map map;
        protected readonly SpriteBatch spriteBatch;

        public Entity(Map map) : base(map.Graphics)
        {
            this.map = map;
            spriteBatch = new SpriteBatch(map.Graphics);

            DebugColor = Color.Orange;
        }

        public override void DebugDraw(Matrix transform)
        {
            base.DebugDraw(transform);
        }
    }
}
