using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class MapSection
    {
        public Rectangle Bounds { get; private set; }
        public List<Tile> Tiles { get; private set; }

        private readonly Map map;

        public MapSection(Map map, Rectangle bounds)
        {
            this.map = map;
            Bounds = bounds;
            Tiles = new List<Tile>();
        }

        public void Draw()
        {
            QuadFragment[] qfs = new QuadFragment[Tiles.Count];

            if (map.Tileset is Tileset tileset)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    var tile = Tiles[i];
                    var tilesetFragment = tileset.GetTilesetFragmentFromIndex(tile.Index);

                    qfs[i] = new QuadFragment
                    {
                        Source = new Rectangle(tilesetFragment.Position, tilesetFragment.Size),
                        Destination = new Rectangle(tile.Position, tile.OverrideSize.HasValue ? tile.OverrideSize.Value : tilesetFragment.Size)
                    };
                }

                map.QuadBatch?.DrawFragments();
            }
        }

        /// <summary>
        /// Create a tile in the section and its corresponding solid in the map if applicable
        /// </summary>
        /// <param name="fragmentIndex"></param>
        /// <param name="position"></param>
        /// <param name="sizeFromTileset"></param>
        /// <param name="overrideSize"></param>
        /// <param name="overrideLayer"></param>
        public void CreateTile(int fragmentIndex, Point position, Point sizeFromTileset, Point? overrideSize = null, int? overrideLayer = null)
        {
            Tiles.Add(new Tile
            {
                Index = fragmentIndex,
                Position = position,
                OverrideSize = overrideSize,
                OverrideLayer = overrideLayer,
            });

            // TODO: Do that only if the fragment is an obstacle!!!
            map.Solids.Add(new Solid(map.Game, new Rectangle(position, overrideSize.HasValue ? overrideSize.Value : sizeFromTileset)));
        }
    }
}
