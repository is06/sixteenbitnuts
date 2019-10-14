using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class Entity : MapElement
    {
        protected readonly Map map;
        protected readonly SpriteBatch spriteBatch;

        public Entity(Map map)
        {
            this.map = map;
            spriteBatch = new SpriteBatch(map.Graphics);
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(Matrix transform)
        {

        }

        public virtual void EditorDraw(Matrix transform)
        {

        }

        public virtual void DebugDraw(Matrix transform)
        {

        }
    }
}
