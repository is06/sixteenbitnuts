using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SixteenBitNuts.Effects;

namespace SixteenBitNuts
{
    public class EffectService
    {
        private Game game;

        public Dictionary<string, MainDisplayEffect> MainDisplayEffects { get; private set; }

        public EffectService(Game game)
        {
            this.game = game;

            MainDisplayEffects = new Dictionary<string, MainDisplayEffect>();
        }

        public void UpdateMainDisplayEffects(RenderTarget2D renderTarget)
        {
            foreach (var mainDisplayEffect in MainDisplayEffects)
            {
                if (mainDisplayEffect.Value.IsEnabled)
                {
                    if (mainDisplayEffect.Value.Effect is Effect effect)
                    {
                        mainDisplayEffect.Value.Update();

                        game.GraphicsDevice.SetRenderTarget(renderTarget);

                        game.SpriteBatch?.Begin(effect: effect);

                        game.SpriteBatch?.Draw(
                            texture: renderTarget,
                            destinationRectangle: new Rectangle(0, 0, (int)game.InternalSize.Width, (int)game.InternalSize.Height),
                            color: Color.White
                        );

                        game.SpriteBatch?.End();
                    }
                }
            }
        }
    }
}
