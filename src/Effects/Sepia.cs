using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class Sepia : MainDisplayEffect
    {
        public Sepia(Game game) : base(game)
        {
            Effect = game.Content.Load<Effect>("Engine/effects/sepia");
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
