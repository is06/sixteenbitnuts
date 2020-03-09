using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Movement
    {
        private readonly Entity entity;
        private Vector2 translation;

        public Movement(Entity entity)
        {
            this.entity = entity;
        }

        public virtual void Update(GameTime gameTime)
        {
            entity.Position += translation;
        }
    }
}
