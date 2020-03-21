using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public class LoopingMovement : Movement
    {
        protected float time = 0f;
        public bool IsLooping { get; set; } = true;

        public LoopingMovement() : base()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            time += (Speed / 100f);
            if (IsLooping && time > 1f) time = 0f;
        }
    }
}
