using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class BouncingMovement : Movement
    {
        public float Strength { get; set; }

        public BouncingMovement() : base()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Translation = new Vector2(
                Strength * Easing.Arch2(time),
                Translation.Y
            );
        }
    }
}
