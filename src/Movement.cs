using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class Movement
    {
        public Vector2 Translation { get; protected set; }
        public float Speed { get; set; }

        protected float time = 0f;

        public Movement()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            time += (Speed / 100f);
            if (time > 1f) time = 0f;
        }
    }
}
