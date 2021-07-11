using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class SmoothStopLinearMovement : EasedLinearMovement
    {
        public float Distance;

        public SmoothStopLinearMovement(float angle) : base(angle)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float oldStep = movementStep;
            if (time < 1f)
            {
                movementStep = (Distance / Speed) * Easing.SmoothStop3(time);
            }
            
            float easingFactor = movementStep - oldStep;

            Translation = Speed * easingFactor * offset;
        }
    }
}
