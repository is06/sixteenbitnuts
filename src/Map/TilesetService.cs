using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class TilesetService
    {
        private readonly Game game;
        public Dictionary<string, Tileset> Tilesets { get; private set; }

        public TilesetService(Game game)
        {
            this.game = game;

            Tilesets = new Dictionary<string, Tileset>();
        }

        public Tileset Get(string name)
        {
            if (Tilesets.ContainsKey(name))
            {
                return Tilesets[name];
            }

            var tileset = new Tileset(game, name);
            Tilesets.Add(name, tileset);
            return tileset;
        }
    }
}
