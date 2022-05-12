using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class Tileset
    {
        private readonly Game game;
        private readonly string name;
        private Dictionary<int, TilesetFragment> fragments;
        private Texture2D? texture;

        public Tileset(Game game, string name)
        {
            this.game = game;
            this.name = name;
            fragments = new Dictionary<int, TilesetFragment>();
        }

        public void Initialize()
        {
            fragments = game.TilesetLoader.LoadFragments(name);
        }

        public void LoadContent()
        {
            texture = game.Content.Load<Texture2D>("Graphics/Tilesets/" + name);
        }

        /// <summary>
        /// Draws a tile from a specified tileset fragment index on the map.
        /// </summary>
        /// <param name="fragmentIndex">Index of the tileset fragment to draw</param>
        /// <param name="position">Position where to draw on the map</param>
        public void DrawTile(int fragmentIndex, Point position)
        {
            if (game.SpriteBatch is SpriteBatch batch)
            {
                var fragmentOrNull = GetTilesetFragmentFromIndex(fragmentIndex);
                if (fragmentOrNull is TilesetFragment fragment)
                {
                    batch.Draw(
                        texture: texture,
                        position: position.ToVector2(),
                        sourceRectangle: new Rectangle(fragment.Position, fragment.Size),
                        color: Color.White,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: Vector2.One,
                        effects: SpriteEffects.None,
                        layerDepth: 0
                    );
                }
            }
        }

        /// <summary>
        /// Gets the index specified tileset fragment.
        /// </summary>
        /// <param name="index">Index of the fragment in the tileset</param>
        /// <returns>The TilesetFragment instance, null if not found</returns>
        public TilesetFragment? GetTilesetFragmentFromIndex(int index)
        {
            if (!fragments.ContainsKey(index))
            {
                return null;
            }
            return fragments[index];
        }
    }
}
