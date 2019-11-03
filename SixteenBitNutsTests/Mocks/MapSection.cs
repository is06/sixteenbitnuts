using Microsoft.Xna.Framework;
using SixteenBitNuts;

namespace SixteenBitNutsTests.Mocks
{
    class MockedMapSection : MapSection
    {
        public MockedMapSection(SixteenBitNuts.Map map, Rectangle bounds, Tileset tileset, string defaultSpawnPointName) : base(map, bounds, tileset, defaultSpawnPointName)
        {
            Entities.Add(defaultSpawnPointName, new MockedSpawnPoint(map, defaultSpawnPointName));
        }

        protected override void LoadTransitionCornerTexture()
        {
            
        }
    }
}
