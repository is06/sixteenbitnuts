using Microsoft.Xna.Framework;
using System.IO;

namespace SixteenBitNuts
{
    public class BinaryMapWriter
    {
        private FileStream? stream;
        private BinaryWriter? writer;

        public void Write(Map map, string filePath)
        {
            stream = File.Open(filePath, FileMode.Create);
            writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, false);

            WriteString("SBNM", 4);

            // Map tileset (32 chars)
            if (!(map.Tileset is Tileset))
            {
                throw new EngineException("Unable to write tileset data into map binary file");
            }
            WriteString(map.Tileset.Name, 32);

            // Section count
            writer.Write(map.Sections.Count);

            foreach (var section in map.Sections)
            {
                WriteSection(section.Value);
            }

            stream.Close();
            writer.Close();
        }

        private void WriteSection(MapSection section)
        {
            // Bounds
            writer?.Write(section.Bounds.Location.X);
            writer?.Write(section.Bounds.Location.Y);
            writer?.Write(section.Bounds.Size.X);
            writer?.Write(section.Bounds.Size.Y);

            // Entity count
            writer?.Write(section.Entities.Count);

            // Entities
            foreach (var entity in section.Entities)
            {
                WriteSectionEntity(entity.Value);
            }

            // Tile count
            writer?.Write(section.Tiles.Count);
            
            // Tiles
            foreach (var tile in section.Tiles)
            {
                WriteSectionTile(tile);
            }
        }

        private void WriteSectionEntity(Entity entity)
        {
            // Type (32 chars)
            WriteString(entity.GetType().ToString(), 32);

            // Position
            writer?.Write(entity.Position.X);
            writer?.Write(entity.Position.Y);
        }

        private void WriteSectionTile(Tile tile)
        {
            // Index
            WriteTileIndex(tile.Index);

            // Position
            writer?.Write(tile.Position.X);
            writer?.Write(tile.Position.Y);

            // Override Size
            if (tile.OverrideSize is Point size)
            {
                // true = size is overrided
                writer?.Write(true);

                // Write the size
                writer?.Write(size.X);
                writer?.Write(size.Y);
            }
            else
            {
                // false = size is not overrided
                writer?.Write(false);

                // No size to write, go to next property
            }

            // Override Layer
            if (tile.OverrideLayer is int layer)
            {
                // true = layer is overrided
                writer?.Write(true);

                // Write the layer
                writer?.Write(layer);
            }
            else
            {
                // false = size is not overrided
                writer?.Write(false);
            }
        }

        private void WriteString(string value, int length)
        {
            char[] buffer = value.PadRight(length, (char)0x0).ToCharArray();
            writer?.Write(buffer);
        }

        private void WriteTileIndex(string index)
        {
            try
            {
                // Index is a number => allocate 4 bytes as an integer
                var intIndex = int.Parse(index);
                writer?.Write(false);
                writer?.Write(intIndex);
            }
            catch
            {
                // Index is a string => allocate 16 bytes for the index
                writer?.Write(true);
                WriteString(index, 16);
            }
        }
    }
}
