using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public abstract class MainDisplayEffect
    {
        public Effect? Effect { get; set; }
        public bool IsEnabled { get; set; }
        public BlendState BlendState { get; set;  }
        public RenderTarget2D RenderTarget { get; private set; }

        protected readonly Game game;

        public MainDisplayEffect(Game game)
        {
            this.game = game;

            BlendState = BlendState.AlphaBlend;
            RenderTarget = new RenderTarget2D(game.GraphicsDevice, (int)game.InternalSize.Width, (int)game.InternalSize.Height);
        }

        public virtual void Update()
        {
            
        }
    }
}
