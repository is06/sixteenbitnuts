using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class EasedLinearMovement : LinearMovement
    {
        protected float time = 0f;

        public EasedLinearMovement(float angle) : base(angle)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            time += (Speed / 100f);
        }
    }
}
