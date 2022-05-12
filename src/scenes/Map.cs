using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public abstract class Map : Scene
    {
        public Tileset? Tileset { get; set; }
        public Dictionary<int, MapSection> Sections { get; private set; }
        public List<Solid> Solids { get; private set; }

        protected Player? player;

        private readonly string name;
        private readonly bool loadFromDefinitionFile;

        public Map(Game game, string name, bool loadFromDefinitionFile = true) : base(game)
        {
            this.name = name;
            this.loadFromDefinitionFile = loadFromDefinitionFile;
            Solids = new List<Solid>();
            Sections = new Dictionary<int, MapSection>();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (loadFromDefinitionFile)
            {
                Game.MapLoader.LoadMapData(this, name);
            }

            player?.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Tileset?.LoadContent();
            player?.LoadContent();
        }

        public override void Update()
        {
            base.Update();

            player?.Update();
        }

        public override void Draw()
        {
            base.Draw();

            foreach (var section in Sections)
            {
                section.Value.Draw(Matrix.Identity);
            }

            player?.Draw(Matrix.Identity);
        }
    }
}
