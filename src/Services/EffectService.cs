using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SixteenBitNuts.Effects;

namespace SixteenBitNuts
{
    public class EffectService
    {
        private readonly Game game;

        public Dictionary<string, MainDisplayEffect> MainDisplayEffects { get; private set; }

        public EffectService(Game game)
        {
            this.game = game;

            MainDisplayEffects = new Dictionary<string, MainDisplayEffect>();
        }

        public RenderTarget2D ApplyEnabledDisplayEffects(RenderTarget2D sourceTarget)
        {
            foreach (var mainDisplayEffect in MainDisplayEffects)
            {
                sourceTarget = ApplyDisplayEffect(sourceTarget, mainDisplayEffect.Value);
            }

            return sourceTarget;
        }

        private RenderTarget2D ApplyDisplayEffect(RenderTarget2D sourceTarget, MainDisplayEffect mainDisplayEffect)
        {
            if (mainDisplayEffect.IsEnabled)
            {
                mainDisplayEffect.Update();

                game.GraphicsDevice.SetRenderTarget(mainDisplayEffect.RenderTarget);

                game.SpriteBatch?.Begin(effect: mainDisplayEffect.Effect, blendState: mainDisplayEffect.BlendState);

                game.SpriteBatch?.Draw(
                    texture: sourceTarget,
                    destinationRectangle: new Rectangle(0, 0, (int)game.InternalSize.Width, (int)game.InternalSize.Height),
                    color: Color.White
                );

                game.SpriteBatch?.End();
            }

            return mainDisplayEffect.RenderTarget;
        }
    }
}
