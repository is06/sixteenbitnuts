using SixteenBitNuts;
using System.IO;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedMap : Map
    {
        public MockedMap(SixteenBitNuts.Game game, string name) : base(game, name)
        {

        }

        protected override void LoadFromFile(string fileName)
        {
            
        }

        protected override void InitPlayer()
        {
            Player = new MockedPlayer(this, new Vector2(0, 0));
        }

        protected override void InitMapEditor()
        {
            
        }

        public void LoadMockFromFile(string fileName)
        {
            string[] lines = File.ReadAllLines("Data/mocks/maps/" + fileName + ".map");

            int sectionIndex = -1;

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "se":
                        // Begin section
                        sectionIndex++;
                        sections[sectionIndex] = new MockedMapSection(
                            this,
                            new Rectangle(
                                int.Parse(components[1]),
                                int.Parse(components[2]),
                                int.Parse(components[3]),
                                int.Parse(components[4])
                            ),
                            new MockedTileset(Game, components[5]),
                            components[6]
                        );
                        break;
                    case "en":
                        // Entities
                        LoadMockedEntity(components[1], sectionIndex, components[2], new Vector2(
                            int.Parse(components[3]),
                            int.Parse(components[4])
                        ));
                        break;
                    case "ti":
                        // Tile
                        int elementId = int.Parse(components[1]);
                        Vector2 position = new Vector2
                        {
                            X = int.Parse(components[2]),
                            Y = int.Parse(components[3])
                        };

                        sections[sectionIndex].Tiles.Add(new MockedTile(
                            this,
                            sections[sectionIndex].Tileset,
                            elementId,
                            position,
                            sections[sectionIndex].Tileset.GetSizeFromId(elementId),
                            sections[sectionIndex].Tileset.GetTypeFromId(elementId)
                        ));
                        break;
                }
            }

            Player.Position = CurrentMapSection.DefaultSpawnPoint.Position;
        }

        private void LoadMockedEntity(string type, int sectionIndex, string name, Vector2 position)
        {
            switch (type)
            {
                case "spawn":
                    sections[sectionIndex].Entities[name] = new MockedSpawnPoint(this, name)
                    {
                        Position = position
                    };
                    break;
            }
        }
    }
}
