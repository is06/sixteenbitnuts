using System.Collections.Generic;

namespace SixteenBitNuts
{
    public interface ITilesetLoader
    {
        Dictionary<int, TilesetFragment> LoadFragments(string name);
    }
}
