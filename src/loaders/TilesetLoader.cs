using System.Collections.Generic;
using System.IO;

namespace SixteenBitNuts
{
    public class TilesetLoader : ITilesetLoader
    {
        public TilesetLoader()
        {

        }

        public string GetTextureFileName(string name)
        {
            return "Graphics/Tilesets/" + name;
        }

        public Dictionary<string, TilesetFragment> LoadFragments(string name)
        {
            var fragments = new Dictionary<string, TilesetFragment>();
            int fragmentIndex = 0;
            string[] lines = File.ReadAllLines("Content/Definitions/Tilesets/" + name + ".tileset");

            foreach (string line in lines)
            {
                string[] chunks = line.Split(' ');

                switch (chunks[0])
                {
                    case "fr":
                        fragments.Add(fragmentIndex.ToString(), CreateFragment(chunks));
                        fragmentIndex++;
                        break;
                }
            }

            return fragments;
        }

        private TilesetFragment CreateFragment(string[] chunks)
        {
            var fragment = new TilesetFragment()
            {
                Position =
                {
                    X = int.Parse(chunks[3]),
                    Y = int.Parse(chunks[4]),
                },
                Size =
                {
                    X = int.Parse(chunks[1]),
                    Y = int.Parse(chunks[2]),
                },
                Type = (TileType)int.Parse(chunks[5])
            };
            return fragment;
        }
    }
}
