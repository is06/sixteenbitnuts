namespace SixteenBitNuts
{
    enum PostProcessEffect
    {
        Glow,
        Glitch
    }

    static class PostProcessEffectExtension
    {
        public static string? GetPath(this PostProcessEffect effect)
        {
            return effect switch
            {
                PostProcessEffect.Glow => "Game/effects/glow",
                PostProcessEffect.Glitch => "Game/effects/glitch",
                _ => null,
            };
        }
    }
}
