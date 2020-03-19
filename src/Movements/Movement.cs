using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Movement
    {
        public Vector2 Translation { get; protected set; }
        public float Speed { get; set; }

        protected float movementStep = 0f;

        public Movement()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            
        }
    }
}
