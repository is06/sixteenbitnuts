﻿using Microsoft.Xna.Framework;
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

            Tileset?.LoadContent();
            Player?.LoadContent();

            LoadQuadBatchContent();
        }

        private void LoadQuadBatchContent()
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

            if (Tileset != null)
            {
                QuadBatch?.LoadContent("Graphics/Tilesets/" + Tileset.Name, qfs);
            }
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

            if (Game.SpriteBatch is SpriteBatch batch)
            {
                // TODO: get the matrix from camera

                foreach (var section in Sections)
                {
                    section.Value.Draw();
                }
            }

            Player?.Draw(Matrix.Identity);
        }

        public override void DebugDraw()
        {
            base.DebugDraw();

            if (Game.SpriteBatch is SpriteBatch batch)
            {
                // TODO: get the matrix from camera
                batch.Begin(transformMatrix: Matrix.Identity);

                Player?.DebugDraw();

                foreach (var solid in Solids)
                {
                    solid.DebugDraw();
                }

                batch.End();
            }
        }
    }
}
