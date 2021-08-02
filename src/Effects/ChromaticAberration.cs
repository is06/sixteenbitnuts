using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class ChromaticAberration : MainDisplayEffect
    {
        public float Offset { get; set; }

        public ChromaticAberration(Game game) : base(game)
        {
            Shader = game.Content.Load<Effect>("EngineGraphics/Effects/chroma");
            Offset = 1f;
        }

        public override void Update()
        {
            base.Update();

            Shader?.Parameters["offset"].SetValue(Offset * 0.002f);
        }
    }
}
