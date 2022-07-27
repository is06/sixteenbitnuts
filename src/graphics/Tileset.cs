using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Tileset
    {
        public string Name { get; private set; }
        public string? TextureFileName { get; private set; }

        private readonly Game game;
        private Dictionary<string, TilesetFragment> fragments;

        public Tileset(Game game, string name)
        {
            Name = name;

            this.game = game;
            fragments = new Dictionary<string, TilesetFragment>();
        }

        public void Initialize()
        {
            if (game.TilesetLoader is ITilesetLoader loader)
            {
                TextureFileName = loader.GetTextureFileName(Name);
                fragments = loader.LoadFragments(Name);
            }
        }

        /// <summary>
        /// Gets the index specified tileset fragment.
        /// </summary>
        /// <param name="index">Index of the fragment in the tileset</param>
        /// <returns>The TilesetFragment instance</returns>
        public TilesetFragment GetTilesetFragmentFromIndex(string index)
        {
            if (!fragments.ContainsKey(index))
            {
                throw new EngineException("No tileset fragment found with index " + index);
            }
            return fragments[index];
        }
    }
}
