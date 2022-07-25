using System.Collections.Generic;

namespace SixteenBitNuts
{
    public interface ISpriteLoader
    {
        public Dictionary<string, SpriteAnimation> LoadAnimations(string name);
    }
}
