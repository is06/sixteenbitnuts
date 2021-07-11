using Microsoft.Xna.Framework;
using System;

namespace SixteenBitNuts
{
    public class LinearMovement : Movement
    {
        protected Vector2 offset;

        public LinearMovement(float angle) : base()
        {
            var x = (float)Math.Cos(angle);
            var y = -(float)Math.Sin(angle);

            offset = new Vector2(x, y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
