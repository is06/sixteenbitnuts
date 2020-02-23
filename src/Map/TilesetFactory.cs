using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class TilesetService
    {
        private readonly Game game;
        private readonly Dictionary<string, Tileset> tilesets = new Dictionary<string, Tileset>();

        public TilesetService(Game game)
        {
            this.game = game;
        }

        public Tileset Get(string name)
        {
            if (tilesets.ContainsKey(name))
            {
                return tilesets[name];
            }

            var tileset = new Tileset(game, name);
            tilesets.Add(name, tileset);
            return tileset;
        }
    }
}
