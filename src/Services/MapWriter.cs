using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SixteenBitNuts
{
    enum MapFileMode
    {
        Binary,
        Text,
        Json
    }

    sealed class MapWriter
    {
        private readonly Map map;

        public MapWriter(Map map)
        {
            this.map = map;
        }

        public void SaveToFile(MapFileMode mode)
        {
            switch (mode)
            {
                case MapFileMode.Binary:
                    SaveInBinaryMode();
                    break;
                case MapFileMode.Text:
                    SaveInTextMode();
                    break;
                case MapFileMode.Json:
                    SaveInJsonMode();
                    break;
            }
        }

        private void SaveInBinaryMode()
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, map);

            string filePath = "Content/Descriptors/Maps/" + map.Name + ".bin";
            File.Delete(filePath);
            File.WriteAllBytes(filePath, stream.ToArray());
        }

        private void SaveInTextMode()
        {
            var contents = new List<string>
            {
                "map " + map.Name
            };

            if (map.Landscape is Landscape landscape)
            {
                contents.Add("bg " + landscape.Name);
                foreach (var layer in landscape.Layers)
                {
                    contents.Add("ly " + layer.Value.Index + " " + layer.Value.Name + " " + layer.Value.TransformOffset.X + " " + layer.Value.TransformOffset.Y);
                }
            }

            foreach (var section in map.Sections)
            {
                contents.Add(section.Value.MapTextDescription);

                foreach (var tilesetSection in section.Value.TilesetSections)
                {
                    contents.Add(tilesetSection.MapTextDescription);

                    foreach (var tile in section.Value.ForegroundTiles)
                    {
                        contents.Add(tile.MapTextDescription);
                    }
                    foreach (var entity in section.Value.Entities)
                    {
                        contents.Add(entity.Value.MapTextDescription);
                    }
                }
            }

            string filePath = "Content/Descriptors/Maps/" + map.Name + ".map";
            File.Delete(filePath);
            File.AppendAllLines(filePath, contents);
        }

        private void SaveInJsonMode()
        {
            
        }
    }
}
