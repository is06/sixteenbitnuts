using System.Collections.Generic;

namespace SixteenBitNuts
{
    public enum AssetType
    {
        Sprite,
        Tileset,
        Map
    }

    public struct Asset
    {
        public AssetType Type;
        public string Name;
    }

    public class AssetManager
    {
        protected List<string> tilesets = new List<string>();
        protected List<string> sprites = new List<string>();

        private readonly Game game;

        private readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Tileset> loadedTilesets = new Dictionary<string, Tileset>();

        public AssetManager(Game game)
        {
            this.game = game;
        }

        public virtual void LoadContent()
        {
            LoadTilesets();
            LoadSprites();
        }

        private void LoadSprites()
        {
            System.Console.WriteLine("Loading sprites...");

            foreach (var sprite in sprites)
            {
                if (sprite is string)
                {
                    LoadSprite(sprite);
                }
            }
        }

        private void LoadTilesets()
        {
            System.Console.WriteLine("Loading tilesets...");

            foreach (var tileset in tilesets)
            {
                if (tileset is string)
                {
                    LoadTileset(tileset);
                }
            }
        }

        private void LoadSprite(string name)
        {
            System.Console.WriteLine("  - " + name + "...");

            var sprite = new Sprite(game, name);
            sprite.Initialize();
            sprite.LoadContent();

            loadedSprites.Add(name, sprite);
        }

        private void LoadTileset(string name)
        {
            System.Console.WriteLine("  - " + name + "...");

            var tileset = new Tileset(game, name);
            tileset.Initialize();
            tileset.LoadContent();

            loadedTilesets.Add(name, tileset);
        }

        public Sprite GetSprite(string name)
        {
            if (!loadedSprites.ContainsKey(name))
            {
                throw new System.Exception("Unable to find sprite '" + name + "'. Did you reference it in the AssetManager loading phase?");
            }

            return loadedSprites[name];
        }

        public Tileset GetTileset(string name)
        {
            if (!loadedTilesets.ContainsKey(name))
            {
                throw new System.Exception("Unable to find tileset '" + name + "'. Did you reference it in the AssetManager loading phase?");
            }

            return loadedTilesets[name];
        }
    }
}
