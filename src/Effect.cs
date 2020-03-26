using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public enum PostProcessEffect
    {
        Glow,
        Glitch,
        Chroma,
        Example
    }

    static class PostProcessEffectExtension
    {
        public static string? GetPath(this PostProcessEffect effect)
        {
            return effect switch
            {
                PostProcessEffect.Glow => "Engine/effects/glow",
                PostProcessEffect.Glitch => "Engine/effects/glitch",
                PostProcessEffect.Chroma => "Engine/effects/chroma",
                PostProcessEffect.Example => "Engine/effects/example",
                _ => null,
            };
        }
    }

    public class EffectService
    {
        public Dictionary<PostProcessEffect, Effect> Effects { get; private set; }

        public EffectService(Game game)
        {
            Effects = new Dictionary<PostProcessEffect, Effect>
            {
                [PostProcessEffect.Chroma] = game.Content.Load<Effect>(PostProcessEffect.Chroma.GetPath()),
                [PostProcessEffect.Example] = game.Content.Load<Effect>(PostProcessEffect.Example.GetPath())
            };
        }

        public void ApplyEffect(PostProcessEffect effect)
        {
            Effects[effect].CurrentTechnique.Passes[0].Apply();
        }
    }
}
