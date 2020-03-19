using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class LinearMovement : Movement
    {
        public LinearMovement() : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Translation = new Vector2(Speed, 0);
        }
    }
}
