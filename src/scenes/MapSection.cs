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

        }

        /// <summary>
        /// Create a tile in the section and its corresponding solid in the map if applicable
        /// </summary>
        /// <param name="fragmentIndex"></param>
        /// <param name="position"></param>
        /// <param name="sizeFromTileset"></param>
        /// <param name="overrideSize"></param>
        /// <param name="overrideLayer"></param>
        public void CreateTile(string fragmentIndex, Point position, Point sizeFromTileset, Point? overrideSize = null, int? overrideLayer = null)
        {
            Tiles.Add(new Tile
            {
                Index = fragmentIndex,
                Position = position,
                OverrideSize = overrideSize,
                OverrideLayer = overrideLayer,
            });

            // TODO: Do that only if the fragment is an obstacle!!!
            map.Solids.Add(new Solid(map.Game, new Rectangle(position, overrideSize ?? sizeFromTileset)));
        }
    }
}
