using Microsoft.Xna.Framework;
using System.IO;

namespace SixteenBitNuts
{
    public class MapLoader : IMapLoader
    {
        public MapLoader()
        {

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
                }
            }
        }

        private void InitTileset(Map map, string[] chunks)
        {
            map.Tileset = new Tileset(map.Game, chunks[1]);
            map.Tileset?.Initialize();
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
            int fragmentIndex = int.Parse(chunks[1]);
            var position = new Point
            {
                X = int.Parse(chunks[2]),
                Y = int.Parse(chunks[3])
            };
            var fragmentOrNull = map.Tileset?.GetTilesetFragmentFromIndex(fragmentIndex);
            if (fragmentOrNull is TilesetFragment fragment)
            {
                section.CreateTile(fragmentIndex, position, fragment.Size);
            }
            else
            {
                throw new System.Exception("Unable to get tileset component size for index " + fragmentIndex);
            }
        }
    }
}
