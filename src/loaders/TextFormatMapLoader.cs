using Microsoft.Xna.Framework;
using System.IO;

namespace SixteenBitNuts
{
    /// <summary>
    /// This map loader is for loading text format maps, meant for quickly testing customizable map definition files.
    /// </summary>
    public class TextFormatMapLoader : IMapLoader
    {
        private EntityFactory? entityFactory;

        public void SetEntityFactory(EntityFactory factory)
        {
            entityFactory = factory;
        }

        public void LoadMapData(Map map, string name)
        {
            string[] lines = File.ReadAllLines("Content/Definitions/Maps/" + name + ".map");
            int sectionIndex = -1;

            foreach (string line in lines)
            {
                string[] chunks = line.Split(' ');

                switch (chunks[0])
                {
                    case "ts":
                        InitTileset(map, chunks);
                        break;
                    case "se":
                        sectionIndex++;
                        BeginSection(map, chunks);
                        break;
                    case "ti":
                        AddTile(map, map.Sections[sectionIndex], chunks);
                        break;
                    case "en":
                        AddEntity(map, map.Sections[sectionIndex], chunks);
                        break;
                }
            }
        }

        private void InitTileset(Map map, string[] chunks)
        {
            map.Tileset = map.Game.AssetManager?.GetTileset(chunks[1]);
        }

        private void BeginSection(Map map, string[] chunks)
        {
            var bounds = new Rectangle(
                int.Parse(chunks[1]),
                int.Parse(chunks[2]),
                int.Parse(chunks[3]),
                int.Parse(chunks[4])
            );

            map.Sections.Add(0, new MapSection(map, bounds));
        }

        /// <summary>
        /// Adds a tile in the specified map section and adds a related solid if any
        /// </summary>
        /// <param name="map"></param>
        /// <param name="section"></param>
        /// <param name="chunks"></param>
        private void AddTile(Map map, MapSection section, string[] chunks)
        {
            string fragmentIndex = chunks[1];
            var position = new Point
            {
                X = int.Parse(chunks[2]),
                Y = int.Parse(chunks[3])
            };
            var fragmentOrNull = map.Tileset?.GetTilesetFragmentFromIndex(fragmentIndex);
            if (fragmentOrNull is TilesetFragment fragment)
            {
                section.CreateTile(fragmentIndex, position, fragment.Size, tileType: fragment.Type);
            }
            else
            {
                throw new System.Exception("Unable to get tileset component size for index " + fragmentIndex);
            }
        }

        private void AddEntity(Map map, MapSection section, string[] chunks)
        {
            var type = chunks[1];
            var name = chunks[2];
            var position = new Point(int.Parse(chunks[3]), int.Parse(chunks[4]));

            var entity = entityFactory?.CreateEntity(map, type, name, position, chunks);
            if (entity is Entity)
            {
                section.Entities.Add(name, entity);
            }
        }
    }
}
