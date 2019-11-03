using Microsoft.Xna.Framework;

namespace SixteenBitNuts
{
    public delegate void TimerHandler();

    public class Timer
    {
        public double Duration { get; set; }
        public bool Active { get; set; }

        public event TimerHandler OnTimerFinished;

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
                    OnTimerFinished?.Invoke();
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
