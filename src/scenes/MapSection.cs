using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class MapSection
    {
        public Rectangle Bounds { get; private set; }
        public List<Tile> Tiles { get; private set; }

        private Map map;

        public MapSection(Map map, Rectangle bounds)
        {
            this.map = map;
            Bounds = bounds;
            Tiles = new List<Tile>();
        }

        public void Draw(Matrix transform)
        {
            map.Game.SpriteBatch?.Begin();

            foreach (var tile in Tiles)
            {
                map.Tileset?.DrawTile(tile.Index, tile.Position);
            }

            map.Game.SpriteBatch?.End();
        }
    }
}
