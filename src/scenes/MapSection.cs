using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class MapSection
    {
        public Rectangle Bounds { get; private set; }
        public List<Tile> Tiles { get; private set; }
        public Dictionary<string, Entity> Entities { get; private set; }

        protected readonly Map map;

        public MapSection(Map map, Rectangle bounds)
        {
            this.map = map;
            Bounds = bounds;
            Tiles = new List<Tile>();
            Entities = new Dictionary<string, Entity>();
        }

        public void Initialize()
        {
            foreach (var entity in Entities)
            {
                entity.Value.Initialize();
            }
        }

        public void LoadContent()
        {
            foreach (var entity in Entities)
            {
                entity.Value.LoadContent();
            }
        }

        public void Update()
        {
            foreach (var entity in Entities)
            {
                entity.Value.Update();
            }
        }

        public void Draw(Matrix transform)
        {
            foreach (var entity in Entities)
            {
                entity.Value.Draw(transform);
            }
        }

        public void DebugDraw(Matrix transform)
        {
            foreach (var entity in Entities)
            {
                entity.Value.DebugDraw(transform);
            }
        }

        /// <summary>
        /// Create a tile in the section and its corresponding solid in the map if applicable
        /// </summary>
        /// <param name="fragmentIndex">Index of the fragment in the tileset</param>
        /// <param name="position">Position of the tile in the map in pixels</param>
        /// <param name="sizeFromTileset"></param>
        /// <param name="overrideSize"></param>
        /// <param name="overrideLayer"></param>
        public void CreateTile(string fragmentIndex, Point position, Point sizeFromTileset, TileType tileType, Point? overrideSize = null, int? overrideLayer = null)
        {
            Tiles.Add(new Tile
            {
                Index = fragmentIndex,
                Position = position,
                OverrideSize = overrideSize,
                OverrideLayer = overrideLayer,
            });

            if (tileType == TileType.Obstacle)
            {
                map.Solids.Add(new Solid(map.Game, new Rectangle(position, overrideSize ?? sizeFromTileset)));
            }
        }

        /// <summary>
        /// Create a basic entity into the map section, override this method to specify a way to create custom entities
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="data"></param>
        public virtual void CreateEntity(string type, string name, Point position, string[] data)
        {
            
        }
    }
}
