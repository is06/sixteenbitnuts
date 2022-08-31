﻿using Microsoft.Xna.Framework;
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

        public MapSection CurrentSection
        {
            get
            {
                return Sections[currentSectionIndex];
            }
        }

        private readonly Camera camera;
        private readonly string name;
        private readonly bool loadFromDefinitionFile;
        private readonly int currentSectionIndex;

        public Map(Game game, string name, bool loadFromDefinitionFile = true) : base(game)
        {
            this.name = name;
            this.loadFromDefinitionFile = loadFromDefinitionFile;
            currentSectionIndex = 0;
            camera = new Camera(this);

            Solids = new List<Solid>();
            Sections = new Dictionary<int, MapSection>();
            QuadBatch = new QuadBatch(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            QuadBatch?.Initialize();

            if (loadFromDefinitionFile)
            {
                Game.MapLoader?.LoadMapData(this, name);
            }

            Player?.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            LoadTilesetContent();
            Player?.LoadContent();
        }

        private void LoadTilesetContent()
        {
            if (Tileset is Tileset tileset && QuadBatch is QuadBatch batch)
            {
                if (tileset.TextureFileName is string texture)
                {
                    batch.LoadContent(texture);
                }
                else
                {
                    throw new EngineException("No tileset texture defined");
                }
            }
        }

        public override void Update()
        {
            base.Update();

            Player?.Update();

            if (Player != null)
            {
                camera.Position = (Player.Position + Player.RelativeCenter).ToVector2();
            }
            camera.Update();

            foreach (var solid in Solids)
            {
                solid.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();

            DrawQuadBatch(camera.Transform);

            foreach (var section in Sections)
            {
                section.Value.Draw();
            }

            Player?.Draw(camera.Transform);
        }

        private void DrawQuadBatch(Matrix transform)
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
                            Destination = new Rectangle(tile.Position, tilesetFragment.Value.Size),
                            IsFlippedHorizontally = tilesetFragment.Value.IsFlippedHorizontally,
                            IsFlippedVertically = tilesetFragment.Value.IsFlippedVertically,
                        };
                        qfi++;
                    }
                }
            }

            QuadBatch?.Draw(qfs, transform);
        }

        public override void DebugDraw()
        {
            base.DebugDraw();

            Player?.DebugDraw(camera.Transform);

            foreach (var solid in Solids)
            {
                solid.DebugDraw(camera.Transform);
            }
        }
    }
}
