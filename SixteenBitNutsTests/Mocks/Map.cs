using SixteenBitNuts;
using Microsoft.Xna.Framework;

namespace SixteenBitNutsTests.Mocks
{
    class MockedMap : SixteenBitNuts.Map
    {
        public MockedMap(SixteenBitNuts.Game game, string name) : base(game, name)
        {

        }

        protected override void LoadFromFile(string fileName)
        {
            sections.Add(0, new MockedMapSection(
                this,
                new Rectangle(0, 0, 480, 270),
                new MockedTileset(
                    new MockedGame(),
                    "test_tileset"
                ),
                "test_default_spawn_point"
            ));
        }

        protected override void InitPlayer()
        {
            Player = new MockedPlayer(this, new Vector2(0, 0));
        }

        protected override void InitMapEditor()
        {
            
        }
    }
}
