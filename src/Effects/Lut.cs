using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Effects
{
    public class Lut : MainDisplayEffect
    {
        private readonly Texture2D lutTexture;

        public Lut(Game game, Texture2D lutTexture) : base(game)
        {
            this.lutTexture = lutTexture;
            Shader = game.Content.Load<Effect>("EngineGraphics/Effects/lut");
        }

        public override void Update()
        {
            base.Update();

            Shader?.Parameters["lutTexture"].SetValue(lutTexture);
        }
    }
}
