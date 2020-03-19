using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class BouncingMovement : LoopingMovement
    {
        public float Strength { get; set; }

        public BouncingMovement() : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float oldStep = movementStep;
            movementStep = Strength * Easing.Arch2(time);

            Translation = new Vector2(0, oldStep - movementStep);
        }
    }
}
