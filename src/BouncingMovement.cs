using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class BouncingMovement : Movement
    {
        public int Strength { get; set; }

        public BouncingMovement(Entity entity) : base(entity)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
