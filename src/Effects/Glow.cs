using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class Glow : MainDisplayEffect
    {
        public Glow(Game game) : base(game)
        {
            Effect = game.Content.Load<Effect>("Engine/effects/glow");
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
