using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class ChromaticAberration : MainDisplayEffect
    {
        public float Offset { get; set; }

        public ChromaticAberration(Game game) : base(game)
        {
            Effect = game.Content.Load<Effect>("EngineGraphics/Effects/chroma");
            Offset = 1f;
        }

        public override void Update()
        {
            base.Update();

            Effect?.Parameters["offset"].SetValue(Offset * 0.002f);
        }
    }
}
