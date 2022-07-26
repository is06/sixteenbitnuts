﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Tileset
    {
        public string Name { get; private set; }

        private readonly Game game;
        private Dictionary<int, TilesetFragment> fragments;

        public Tileset(Game game, string name)
        {
            Name = name;

            this.game = game;
            fragments = new Dictionary<int, TilesetFragment>();
        }

        public void Initialize()
        {
            if (game.TilesetLoader is ITilesetLoader loader)
            {
                fragments = loader.LoadFragments(Name);
            }
        }

        public void LoadContent()
        {

        }

        /// <summary>
        /// Gets the index specified tileset fragment.
        /// </summary>
        /// <param name="index">Index of the fragment in the tileset</param>
        /// <returns>The TilesetFragment instance</returns>
        public TilesetFragment GetTilesetFragmentFromIndex(int index)
        {
            if (!fragments.ContainsKey(index))
            {
                throw new EngineException("No tileset fragment found with index " + index);
            }
            return fragments[index];
        }
    }
}
