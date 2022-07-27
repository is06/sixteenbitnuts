using System.Collections.Generic;

namespace SixteenBitNuts
{
    public interface ITilesetLoader
    {
        string GetTextureFileName(string name);
        Dictionary<string, TilesetFragment> LoadFragments(string name);
    }
}
