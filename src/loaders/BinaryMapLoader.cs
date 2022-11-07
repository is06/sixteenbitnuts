using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class BinaryMapLoader : IMapLoader
    {
        private EntityFactory? entityFactory;
        private BinaryReader reader;

        public string GetBinaryPath()
        {
            return "Content/Data/Maps";
        }

        public void SetEntityFactory(EntityFactory factory)
        {
            entityFactory = factory;
        }

        public void LoadMapData(Map map, string name)
        {
            var stream = File.Open(GetBinaryPath() + "/" + name + ".bin", FileMode.Open);
            reader = new BinaryReader(stream);

            // Reading file header
            reader.ReadChars(4);

            // Tileset
            InitTileset(map, ReadString(32));

            // Section count
            var sectionCount = reader.ReadInt32();
            for (int i = 0; i < sectionCount; i++)
            {
                AddSection(map);
            }
        }

        private void InitTileset(Map map, string name)
        {
            map.Tileset = map.Game.AssetManager?.GetTileset(name);
        }

        private void AddSection(Map map)
        {
            var mapSectionBounds = Rectangle.Empty;

            // Bounds
            mapSectionBounds.Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            mapSectionBounds.Size = new Point(reader.ReadInt32(), reader.ReadInt32());

            // Create the section object
            var section = new MapSection(map, mapSectionBounds);

            // Entity count
            var entityCount = reader.ReadInt32();

            // Entities

            // Tile count
            var tileCount = reader.ReadInt32();

            // Tiles
            for (int i = 0; i < tileCount; i++)
            {
                AddTile(map, section);
            }

            map.Sections.Add(0, section);
        }

        private void AddTile(Map map, MapSection section)
        {
            var bounds = Rectangle.Empty;
            int? overrideLayer = null;

            // Index
            var isIndexAString = reader.ReadBoolean();
            var pattern = isIndexAString ? ReadString(16) : reader.ReadInt32().ToString();

            // Position
            bounds.Location = new Point(reader.ReadInt32(), reader.ReadInt32());

            // Override Size
            var isSizeOverrided = reader.ReadBoolean();
            if (isSizeOverrided)
            {
                bounds.Size = new Point(reader.ReadInt32(), reader.ReadInt32());
            }

            // Override Layer
            var isLayerOverrided = reader.ReadBoolean();
            if (isLayerOverrided)
            {
                overrideLayer = reader.ReadInt32();
            }

            var fragment = map.Tileset?.GetTilesetFragmentFromIndex(pattern);
            if (fragment is TilesetFragment)
            {
                section.CreateTile(
                    fragmentIndex: pattern,
                    position: bounds.Location,
                    sizeFromTileset: fragment.Value.Size,
                    overrideSize: bounds.Size,
                    overrideLayer: overrideLayer
                );
            }
        }

        private string ReadString(int length)
        {
            var chars = new string(reader.ReadChars(32));
            return chars.Trim(new char[] { '\0' });
        }
    }
}
