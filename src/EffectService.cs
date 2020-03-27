using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SixteenBitNuts.Effects;

namespace SixteenBitNuts
{
    public class EffectService
    {
        public Dictionary<string, MainDisplayEffect> MainDisplayEffects { get; private set; }

        public EffectService(Game game)
        {
            MainDisplayEffects = new Dictionary<string, MainDisplayEffect>
            {
                ["chroma"] = new ChromaticAberration(game),
                ["glow"] = new Glow(game),
            };
        }

        public void UpdateMainDisplayEffects()
        {
            foreach (var mainDisplayEffect in MainDisplayEffects)
            {
                if (mainDisplayEffect.Value.IsEnabled)
                {
                    if (mainDisplayEffect.Value.Effect is Effect effect)
                    {
                        mainDisplayEffect.Value.Update();
                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                        }
                    }
                }
            }
        }
    }
}
