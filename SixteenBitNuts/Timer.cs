using System;
using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    class Timer
    {
        public double Duration { get; set; }
        public bool Active { get; set; }
        public Action Callback { get; set; }

        private double elapsed;

        public Timer()
        {
            elapsed = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                elapsed += gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsed >= Duration)
                {
                    Callback.Invoke();
                    Active = false;
                    elapsed = 0;
                }
            }
        }

        public void Restart()
        {
            elapsed = 0;
            Active = true;
        }
    }
}
