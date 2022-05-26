using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public abstract class Map : Scene
    {
        public Tileset? Tileset { get; set; }
        public QuadBatch? QuadBatch { get; private set; }
        public Dictionary<int, MapSection> Sections { get; private set; }
        public List<Solid> Solids { get; private set; }
        public Player? Player { get; protected set; }

        private readonly string name;
        private readonly bool loadFromDefinitionFile;

        public Map(Game game, string name, bool loadFromDefinitionFile = true) : base(game)
        {
            this.name = name;
            this.loadFromDefinitionFile = loadFromDefinitionFile;
            Solids = new List<Solid>();
            Sections = new Dictionary<int, MapSection>();
            QuadBatch = new QuadBatch(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (loadFromDefinitionFile)
            {
                Game.MapLoader?.LoadMapData(this, name);
            }

            Player?.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (Tileset is Tileset tileset && QuadBatch is QuadBatch batch)
            {
                tileset.LoadContent();
                batch.LoadContent("Graphics/Tilesets/" + tileset.Name);
            }
            
            Player?.LoadContent();
        }

        public override void Update()
        {
            base.Update();

            Player?.Update();

            foreach (var solid in Solids)
            {
                solid.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();

            DrawQuadBatch();

            foreach (var section in Sections)
            {
                section.Value.Draw();
            }

            Player?.Draw(Matrix.Identity);
        }

        private void DrawQuadBatch()
        {
            int tileCount = 0;
            foreach (var section in Sections)
            {
                tileCount += section.Value.Tiles.Count;
            }

            QuadFragment[] qfs = new QuadFragment[tileCount];
            int qfi = 0;
            foreach (var section in Sections)
            {
                foreach (var tile in section.Value.Tiles)
                {
                    var tilesetFragment = Tileset?.GetTilesetFragmentFromIndex(tile.Index);
                    if (tilesetFragment.HasValue)
                    {
                        qfs[qfi] = new QuadFragment
                        {
                            Source = new Rectangle(tilesetFragment.Value.Position, tilesetFragment.Value.Size),
                            Destination = new Rectangle(tile.Position, tilesetFragment.Value.Size)
                        };
                        qfi++;
                    }
                }
            }

            QuadBatch?.Draw(qfs);
        }

        public override void DebugDraw()
        {
            base.DebugDraw();

            Player?.DebugDraw();

            foreach (var solid in Solids)
            {
                solid.DebugDraw();
            }
        }
    }
}
