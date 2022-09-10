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
        protected List<Asset> assetsToLoad = new List<Asset>();
        protected readonly Game game;

        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private Dictionary<string, Tileset> tilesets = new Dictionary<string, Tileset>();

        public AssetManager(Game game)
        {
            this.game = game;
        }

        public virtual void LoadContent()
        {
            foreach (var asset in assetsToLoad)
            {
                switch (asset.Type)
                {
                    case AssetType.Sprite:
                        LoadSprite(asset.Name);
                        break;
                    case AssetType.Tileset:
                        LoadTileset(asset.Name);
                        break;
                    default:
                        throw new System.Exception("Unsupported asset type (" + asset.Type.ToString() + ") in AssetManager loading phase");
                }
            }
        }

        private void LoadSprite(string name)
        {
            var sprite = new Sprite(game, name);
            sprite.Initialize();
            sprite.LoadContent();

            sprites.Add(name, sprite);
        }

        private void LoadTileset(string name)
        {
            var tileset = new Tileset(game, name);
            tileset.Initialize();
            tileset.LoadContent();

            tilesets.Add(name, tileset);
        }

        public Sprite GetSprite(string name)
        {
            if (!sprites.ContainsKey(name))
            {
                throw new System.Exception("Unable to find sprite '" + name + "'. Did you reference it in the AssetManager loading phase?");
            }

            return sprites[name];
        }

        public Tileset GetTileset(string name)
        {
            if (!tilesets.ContainsKey(name))
            {
                throw new System.Exception("Unable to find tileset '" + name + "'. Did you reference it in the AssetManager loading phase?");
            }

            return tilesets[name];
        }
    }
}
