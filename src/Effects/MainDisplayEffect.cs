using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public abstract class MainDisplayEffect
    {
        public Effect? Effect { get; set; }
        public bool IsEnabled { get; set; }

        protected readonly Game game;

        public MainDisplayEffect(Game game)
        {
            this.game = game;
        }

        public virtual void Update()
        {
            
        }
    }
}
