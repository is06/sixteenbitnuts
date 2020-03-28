using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class Glow : MainDisplayEffect
    {
        public float Amount { get; set; }

        public Glow(Game game) : base(game)
        {
            Effect = game.Content.Load<Effect>("Engine/effects/glow");
            Amount = 1f;
            BlendState = BlendState.Additive;
        }

        public override void Update()
        {
            base.Update();

            Effect?.Parameters["amount"].SetValue(Amount * 0.003f);
        }
    }
}
